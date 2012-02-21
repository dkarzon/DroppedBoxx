using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DroppedBoxx.Code;
using DroppedBoxx.Code.Responses;
using DroppedBoxx.Models;
using Fluid.Controls;

namespace DroppedBoxx.Sync
{
    public class SyncThread
    {
        public void Go()
        {
            //Syncing does here
            //foreach folder in sync folders upload the files...?
            foreach (var syncFolder in Settings.Instance.SyncFolders)
            {
                if (!Directory.Exists(syncFolder.Path)) continue;

                Form1.Instance.ShowSyncStatus();
                //add all the files in this folder
                Sync(syncFolder, "/DroppedBoxx");

                //Set the last Sync on the Sync Folder
                syncFolder.LastSync = DateTime.Now;
            }

            Finish();

            //Show the syncing complete thingy
            Form1.Instance.ShowSyncStatus();
        }

        private void Sync(SyncFolder syncFolder, string dropPath)
        {
            //Try create the folder...
            var parentMeta = new MetaData { Path = dropPath };

            var tempMeta = new MetaData { Path = string.Format("{0}/{1}", dropPath, syncFolder.Name) };

            var folderMeta = Syncer.Instance.DropBox.GetItems(tempMeta);

            if (folderMeta == null)
            {
                //Doesnt exist on Dropbox
                //if its the Sync Root Create it anyway...
                if (syncFolder.Parent == null)
                {
                    folderMeta = Syncer.Instance.DropBox.CreateFolder(parentMeta, syncFolder.Name);
                }
                else
                {
                    //Not the root SyncFolder 
                    if (syncFolder.LastSync == DateTime.MinValue)
                    {
                        //not been synced before (is new)
                        folderMeta = Syncer.Instance.DropBox.CreateFolder(parentMeta, syncFolder.Name);
                    }
                    else
                    {
                        //has been synced before
                        if (syncFolder.Created > syncFolder.LastSync)
                        {
                            //Created after last sync, new
                            folderMeta = Syncer.Instance.DropBox.CreateFolder(parentMeta, syncFolder.Name);
                        }
                        else
                        {
                            //Folder deleted from dropbox after last sync
                            try
                            {
                                Directory.Delete(syncFolder.Path);
                            }
                            catch { }
                            return; //End current loop
                        }
                    }
                }

                //if folderMeta is still null, data fail
                if (folderMeta == null)
                {
                    Form1.Instance.DoAction(() =>
                    {
                        MessageDialog.Show("Syncing failed" + Environment.NewLine + "Check Data Connection", "OK", null);
                    });
                    return;
                }
            }

            if (folderMeta.Contents == null) folderMeta.Contents = new List<MetaData>();

            var folders = syncFolder.GetSubFolders();

            foreach (SyncFolder subFolder in folders)
            {
                subFolder.Parent = syncFolder;
                Sync(subFolder, string.Format("{0}/{1}", dropPath, syncFolder.Name));
            }

            Syncer.Instance.CurrentFolderPath = syncFolder.Path;

            var files = syncFolder.GetFiles();

            foreach (SyncFile file in files)
            {
                try
                {
                    UpdateSyncStatuses(file.Name, SyncStatus.Checking);
                    //Check for extension and size exclusions
                    if (Settings.Instance.MaxSizeMB > 0)
                    {
                        if (Settings.Instance.MaxSizeMB < (file.Size / 1024.00 / 1024.00))
                        {
                            //File too big...
                            //Add to the Syncer thing
                            //Syncer.Instance.FilesSkipped.Add(file);
                            continue;
                        }
                    }
                    //Check file against the dropbox 1
                    var fileMeta = folderMeta.Contents.SingleOrDefault(f => f.Name == file.Name);
                    if (fileMeta != null)
                    {
                        //file exists on dropbox check if modified after last sync
                        if (syncFolder.LastSync == DateTime.MinValue)
                        {
                            //First Time syncing said folder and file exists on both...
                            if (fileMeta.Modified < TimeZone.CurrentTimeZone.ToUniversalTime(file.LastMod))
                            {
                                //phone file is newer (upload)
                                UpdateSyncStatuses(SyncStatus.Uploading);
                                Syncer.Instance.DropBox.UploadFile(new FileInfo(file.Path), folderMeta);
                                //Add to the Syncer thing
                                //Syncer.Instance.FilesUp.Add(file);
                            }
                            else
                            {
                                //update phone files (download)   new FileInfo(file.Path), folderMeta
                                UpdateSyncStatuses(SyncStatus.Downloading);
                                var downloadFile = Syncer.Instance.DropBox.GetFile(fileMeta);

                                if (downloadFile != null)
                                {
                                    downloadFile.SaveFile(file.Path);
                                    //Add to the Syncer thing
                                    //Syncer.Instance.FilesDown.Add(file);
                                }
                            }
                        }
                        else
                        {
                            //Not first time syncing folder
                            if (fileMeta.Modified > TimeZone.CurrentTimeZone.ToUniversalTime(file.LastMod))
                            {
                                if (fileMeta.Modified > TimeZone.CurrentTimeZone.ToUniversalTime(syncFolder.LastSync))
                                {
                                    //update phone files (download)   new FileInfo(file.Path), folderMeta
                                    UpdateSyncStatuses(SyncStatus.Downloading);
                                    var downloadFile = Syncer.Instance.DropBox.GetFile(fileMeta);

                                    if (downloadFile != null)
                                    {
                                        downloadFile.SaveFile(file.Path);
                                        //Add to the Syncer thing
                                        //Syncer.Instance.FilesDown.Add(file);
                                    }
                                }
                            }
                            else
                            {
                                if (file.LastMod > syncFolder.LastSync) //Both Local
                                {
                                    //phone file is newer (upload)
                                    UpdateSyncStatuses(SyncStatus.Uploading);
                                    Syncer.Instance.DropBox.UploadFile(new FileInfo(file.Path), folderMeta);
                                    //Add to the Syncer thing
                                    //Syncer.Instance.FilesUp.Add(file);
                                }
                            }

                        }
                    }
                    else
                    {
                        //File doesnt exist on Dropbox
                        //check if it was deleted
                        if (file.LastMod > syncFolder.LastSync) //Both Local
                        {
                            //file added after last sync (upload)
                            UpdateSyncStatuses(SyncStatus.Uploading);
                            Syncer.Instance.DropBox.UploadFile(new FileInfo(file.Path), folderMeta);
                            //Add to the Syncer thing
                            //Syncer.Instance.FilesUp.Add(file);
                        }
                        else
                        {
                            //Check for moved file...
                            if (file.LastAccess > syncFolder.LastSync)
                            {
                                //TODO - TESTING!

                                //File was moved? upload it to dropbox...
                                UpdateSyncStatuses(SyncStatus.Uploading);
                                Syncer.Instance.DropBox.UploadFile(new FileInfo(file.Path), folderMeta);
                            }
                            else
                            {
                                //Delete Local File
                                try
                                {
                                    File.Delete(file.Path);
                                }
                                catch
                                {
                                    //Failed to delete.
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //Pokemon Exception handling...
                }
            }

            //reload files list for deletes
            files = syncFolder.GetFiles();

            //Now check for files the are on dropbox but not phone
            var syncFiles = new List<SyncFile>();
            foreach (SyncFile file in files)
            {
                syncFiles.Add(file);
            }

            var newFiles = from df in folderMeta.Contents
                join lf in syncFiles on df.Name equals lf.Name into nf
                from f in nf.DefaultIfEmpty()
                where f == null && (!df.Is_Dir)
                select df;

            foreach (MetaData dropMeta in newFiles)
            {
                UpdateSyncStatuses(dropMeta.Name, SyncStatus.Checking);
                //Check for extension and size exclusions
                if (Settings.Instance.MaxSizeMB > 0)
                {
                    if (Settings.Instance.MaxSizeMB < (dropMeta.Bytes / 1024.00 / 1024.00))
                    {
                        //File too big...
                        //Add to the Syncer thing
                        //Syncer.Instance.FilesSkippedD.Add(dropFile);
                        continue;
                    }
                }
                //check if dropbox file was added after last sync
                if (dropMeta.Modified > TimeZone.CurrentTimeZone.ToUniversalTime(syncFolder.LastSync))
                {
                    //new file (download)
                    UpdateSyncStatuses(SyncStatus.Downloading);
                    var downloadFile = Syncer.Instance.DropBox.GetFile(dropMeta);

                    if (downloadFile != null)
                    {
                        downloadFile.SaveFile(Path.Combine(syncFolder.Path, downloadFile.Name));
                        //Add to the Syncer thing
                        //Syncer.Instance.FilesNew.Add(dropFile);
                    }
                }
                else
                {
                    //file deleted (delete)
                    Syncer.Instance.DropBox.Delete(dropMeta);
                }
            }

            //now check for folders on dropbox but not the phone
            var syncFolders = new List<SyncFolder>();
            foreach (SyncFolder folder in folders)
            {
                syncFolders.Add(folder);
            }

            var newFolders = from df in folderMeta.Contents
                             join lf in syncFolders on df.Name equals lf.Name into nf
                             from f in nf.DefaultIfEmpty()
                             where f == null && df.Is_Dir
                             select df;

            foreach (MetaData dropMeta in newFolders)
            {
                //ITS A FOLDER! (exists on dropbox but not phone)

                //If folder was added to dropbox after last sync, download it
                if (dropMeta.Modified > TimeZone.CurrentTimeZone.ToUniversalTime(syncFolder.LastSync))
                {
                    //create local folder and download files in said folder
                    var newFolder = new DirectoryInfo(Path.Combine(syncFolder.Path, dropMeta.Name));
                    if (!newFolder.Exists) newFolder.Create();

                    var subFolder = new SyncFolder(newFolder);
                    subFolder.Parent = syncFolder;
                    Sync(subFolder, string.Format("{0}/{1}", dropPath, syncFolder.Name));
                }
                else
                {
                    //folder deleted (delete)
                    Syncer.Instance.DropBox.Delete(dropMeta);
                }
                continue;
            }
        }

        private void UpdateSyncStatuses(SyncStatus syncStatus)
        {
            UpdateSyncStatuses(null, syncStatus);
        }

        private void UpdateSyncStatuses(string fileName, SyncStatus syncStatus)
        {
            //Set the syncer things
            if (!string.IsNullOrEmpty(fileName)) Syncer.Instance.CurrentFileName = fileName;
            Syncer.Instance.SyncStatus = syncStatus;
            //GUI AWAY!
            Form1.Instance.ShowSyncStatus();
        }

        public void Finish()
        {
            Syncer.Instance.SyncFinished();
        }

    }
}