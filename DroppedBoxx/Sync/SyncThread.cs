using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DroppedBoxx.Code;
using DroppedBoxx.Code.Responses;
using DroppedBoxx.Models;
using Fluid.Controls;
using DroppedBoxx.ViewModels;
using DroppedBoxx.Enums;

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
                try
                {
                    Sync(syncFolder, "/DroppedBoxx");
                }
                catch (Exception ex)
                {
                    //Maybe try send this somewhere?
                }

                //Set the last Sync on the Sync Folder
                syncFolder.LastSync = DateTime.Now;
            }

            Finish();

            //Show the syncing complete thingy
            Form1.Instance.ShowSyncStatus();
        }

        private void Sync(ItemViewModel syncFolder, string dropPath)
        {
            //Sync Status
            UpdateSyncStatuses(syncFolder.Name, SyncStatus.Checking);

            //Parent temp Meta
            var parentMeta = new MetaData { Path = dropPath };

            //get all the files in the dropbox folder
            var tempMeta = new MetaData { Path = string.Format("{0}/{1}", dropPath, syncFolder.Name) };
            var dropboxFolder = Syncer.Instance.DropBox.GetItems(tempMeta);
            
            
            ////if the dropboxfolder is null, does not exist on dropbox
            if (dropboxFolder == null)
            {
                //if its the Sync Root Create it anyway...
                if (syncFolder.ParentFolder == null || syncFolder.LastSync == null)
                {
                    UpdateSyncStatuses(syncFolder.Name, SyncStatus.Uploading);
                    //is root or hasnt been synced yet
                    dropboxFolder = Syncer.Instance.DropBox.CreateFolder(parentMeta, syncFolder.Name);
                    syncFolder.LastSync = DateTime.UtcNow;
                    syncFolder.LastResult = SyncResultEnum.Uploaded;
                }
                else
                {
                    //not the sync root or has been synced previously
                    //Folder deleted from dropbox after last sync
                    try
                    {
                        syncFolder.LastSync = DateTime.UtcNow;
                        syncFolder.LastResult = SyncResultEnum.Deleted;
                        Directory.Delete(syncFolder.Path);
                    }
                    catch { }
                    return; //End current loop
                }

                //if folderMeta is still null, data fail
                if (dropboxFolder == null)
                {
                    Form1.Instance.DoAction(() =>
                    {
                        MessageDialog.Show("Syncing failed" + Environment.NewLine + "Check Data Connection", "OK", null);
                    });
                    return;
                }
            }

            //Fix for null contents
            if (dropboxFolder.Contents == null) dropboxFolder.Contents = new List<MetaData>();

            //get all the files in the device folder
            var deviceFolder = new DirectoryInfo(syncFolder.Path);
            var deviceFiles = deviceFolder.GetFiles();
            //get all the files in the folder model
            
            ////Compare the dropbox file to the device folder
            ////check the last sync date from the model
            foreach (var deviceFile in deviceFiles)
            {
                UpdateSyncStatuses(deviceFile.Name, SyncStatus.Checking);
                //Check for extension and size exclusions
                if (Settings.Instance.MaxSizeMB > 0)
                {
                    if (Settings.Instance.MaxSizeMB < (deviceFile.Length / 1024.00 / 1024.00))
                    {
                        //File too big...
                        continue;
                    }
                }

                var dropboxfile = dropboxFolder.Contents.SingleOrDefault(f => f.Name == deviceFile.Name);
                var modelFile = syncFolder.Files.SingleOrDefault(f => f.Name == deviceFile.Name);

                if (dropboxfile == null)
                {
                    //file doesnt exist on Dropbox
                    if (modelFile == null)
                    {
                        //hasnt been synced before (upload)
                        UpdateSyncStatuses(deviceFile.Name, SyncStatus.Uploading);
                        Syncer.Instance.DropBox.UploadFile(deviceFile, dropboxFolder);

                        //create the new modelfile
                        modelFile = new ItemViewModel(deviceFile, syncFolder)
                        {
                            LastSync = DateTime.UtcNow,
                            LastResult = SyncResultEnum.Uploaded
                        };
                        syncFolder.Files.Add(modelFile);
                    }
                    else if (modelFile.LastSync == null)
                    {
                        UpdateSyncStatuses(deviceFile.Name, SyncStatus.Uploading);
                        Syncer.Instance.DropBox.UploadFile(deviceFile, dropboxFolder);

                        modelFile.LastResult = SyncResultEnum.Uploaded;
                        modelFile.LastSync = DateTime.UtcNow;
                    }
                    else
                    {
                        //folder has been previously synced, must have been deleted from Dropbox (delete)
                        try
                        {
                            deviceFile.Delete();
                        }
                        catch{}
                    }
                }
                else
                {
                    //exists on dropbox
                    if (modelFile == null)
                    {
                        //Possible conflict?
                        if (dropboxfile.Modified > TimeZone.CurrentTimeZone.ToUniversalTime(deviceFile.LastWriteTime))
                        {
                            //dropbox file newer (download)
                            UpdateSyncStatuses(SyncStatus.Downloading);
                            var downloadFile = Syncer.Instance.DropBox.GetFile(dropboxfile);

                            if (downloadFile != null)
                            {
                                downloadFile.SaveFile(deviceFile.FullName);

                                modelFile = new ItemViewModel(deviceFile, syncFolder)
                                {
                                    LastResult = SyncResultEnum.Downloaded,
                                    LastSync = DateTime.UtcNow
                                };
                                syncFolder.Files.Add(modelFile);
                            }

                        }
                        else
                        {
                            //local file newer (upload)
                            UpdateSyncStatuses(deviceFile.Name, SyncStatus.Uploading);
                            Syncer.Instance.DropBox.UploadFile(deviceFile, dropboxFolder);

                            modelFile.LastResult = SyncResultEnum.Uploaded;
                            modelFile.LastSync = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        //check the sync history for the last sync date
                        if (dropboxfile.Modified > modelFile.LastSync)
                        {
                            //dropbox file newer (download)
                            UpdateSyncStatuses(deviceFile.Name, SyncStatus.Downloading);
                            var downloadFile = Syncer.Instance.DropBox.GetFile(dropboxfile);

                            if (downloadFile != null)
                            {
                                downloadFile.SaveFile(deviceFile.FullName);

                                modelFile.LastResult = SyncResultEnum.Downloaded;
                                modelFile.LastSync = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            //dropbox file older than last sync
                            if (TimeZone.CurrentTimeZone.ToUniversalTime(deviceFile.LastWriteTime) > modelFile.LastSync)
                            {
                                //local file has been updated (upload)
                                UpdateSyncStatuses(deviceFile.Name, SyncStatus.Uploading);
                                Syncer.Instance.DropBox.UploadFile(deviceFile, dropboxFolder);

                                modelFile.LastResult = SyncResultEnum.Uploaded;
                                modelFile.LastSync = DateTime.UtcNow;
                            }
                        }
                    }
                }//if (dropboxfile == null)
            }//foreach (var deviceFile in deviceFiles)

            //reload deviceFiles variable to get deletes
            deviceFiles = deviceFolder.GetFiles();

            //Get all the dropbox files that dont 
            var dropboxOnlyFiles = from df in dropboxFolder.Contents
                                   join lf in deviceFiles on df.Name equals lf.Name into nf
                                   from f in nf.DefaultIfEmpty()
                                   where f == null && (!df.Is_Dir)
                                   select df;

            foreach (var dropboxFile in dropboxOnlyFiles)
            {
                UpdateSyncStatuses(dropboxFile.Name, SyncStatus.Checking);
                //Check for extension and size exclusions
                if (Settings.Instance.MaxSizeMB > 0)
                {
                    if (Settings.Instance.MaxSizeMB < (dropboxFile.Bytes / 1024.00 / 1024.00))
                    {
                        //File too big...
                        continue;
                    }
                }

                var modelFile = syncFolder.Files.SingleOrDefault(f => f.Name == dropboxFile.Name);

                //file exists on dropbox but not device
                if (modelFile == null)
                {
                    //has not been synced before (download)
                    UpdateSyncStatuses(dropboxFile.Name, SyncStatus.Downloading);
                    var downloadFile = Syncer.Instance.DropBox.GetFile(dropboxFile);

                    if (downloadFile != null)
                    {
                        var filePath = Path.Combine(syncFolder.Path, downloadFile.Name);
                        downloadFile.SaveFile(filePath);

                        modelFile = new ItemViewModel(new FileInfo(filePath), syncFolder)
                        {
                            LastResult = SyncResultEnum.Downloaded,
                            LastSync = DateTime.UtcNow
                        };
                        syncFolder.Files.Add(modelFile);
                    }
                }
                else
                {
                    //has been synced before (delete?)
                    Syncer.Instance.DropBox.Delete(dropboxFile);
                    modelFile.LastSync = DateTime.UtcNow;
                    modelFile.LastResult = SyncResultEnum.Deleted;
                }

            }//foreach (var dropboxFile in dropboxOnlyFiles)



            //Now do folders...
            foreach (var deviceSubFolder in deviceFolder.GetDirectories())
            {
                var modelFolder = syncFolder.SubFolders.SingleOrDefault(f => f.Name == deviceSubFolder.Name);

                if (modelFolder == null)
                {
                    //new syncer
                    modelFolder = new ItemViewModel(deviceSubFolder, syncFolder);
                    syncFolder.SubFolders.Add(modelFolder);
                }
                else
                {
                    //Set the parent folder again.
                    modelFolder.ParentFolder = syncFolder;
                }

                Sync(modelFolder, string.Format("{0}/{1}", dropPath, syncFolder.Name));
                modelFolder.LastSync = DateTime.UtcNow;
            }

            var deviceSubFolders = deviceFolder.GetDirectories();

            var dropboxOnlyFolders = from df in dropboxFolder.Contents
                                     join lf in deviceSubFolders on df.Name equals lf.Name into nf
                                     from f in nf.DefaultIfEmpty()
                                     where f == null && df.Is_Dir
                                     select df;

            foreach (var dropboxSubFolder in dropboxOnlyFolders)
            {
                //Folder exists on dropbox and not locally
                var modelFolder = syncFolder.SubFolders.SingleOrDefault(f => f.Name == dropboxSubFolder.Name);

                var folderPath = Path.Combine(syncFolder.Path, dropboxSubFolder.Name);
                var newDirInfo = new DirectoryInfo(folderPath);

                if (modelFolder == null)
                {
                    //new folder from Dropbox

                    if (!newDirInfo.Exists) newDirInfo.Create();

                    modelFolder = new ItemViewModel(newDirInfo, syncFolder);
                    modelFolder.LastResult = SyncResultEnum.Downloaded;
                    syncFolder.SubFolders.Add(modelFolder);
                }
                else
                {
                    if (dropboxSubFolder.Modified > modelFolder.LastSync)
                    {
                        //Folder added after last sync
                        //Set the parent folder again.
                        modelFolder.ParentFolder = syncFolder;
                    }
                    else
                    {
                        //folder deleted (delete)
                        Syncer.Instance.DropBox.Delete(dropboxSubFolder);
                        modelFolder.LastSync = DateTime.UtcNow;
                        modelFolder.LastResult = SyncResultEnum.Deleted;
                        continue;
                    }
                }

                Sync(modelFolder, string.Format("{0}/{1}", dropPath, syncFolder.Name));
                modelFolder.LastSync = DateTime.UtcNow;
            }

            //now remove the model files and folders that were deleted?
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