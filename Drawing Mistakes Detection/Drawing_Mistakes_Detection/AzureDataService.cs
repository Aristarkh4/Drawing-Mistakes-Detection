using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing_Mistakes_Detection
{
    class AzureDataService
    {
        public MobileServiceClient MobileService { get; set; }
        IMobileServiceSyncTable<DrawingWithTag> drawingswithtagsTable;

        public async Task Initialize()
        {
            //Create client
            MobileService = new MobileServiceClient("http://drawingmistakesdetection.azurewebsites.net");

            const string path = "syncstore.db";
            //Setup local sqlite store and initialize table
            var store = new MobileServiceSQLiteStore(path);
            store.DefineTable<DrawingWithTag>();
            await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get sync table that will call out to azure
            drawingswithtagsTable = MobileService.GetSyncTable<DrawingWithTag>();

            Debug.WriteLine("finishedinitialization");
        }

        public async Task<IEnumerable> GetDrawingsWithTags()
        {
            await SyncDrawingsWithTags();
            return await drawingswithtagsTable.OrderBy(c => c.DateUtc).ToEnumerableAsync();
        }

        public async Task AddDrawingWithTag(byte[] tagIds)
        {
            //create and insert drawing with separate tags
            foreach (byte tagId in tagIds)
            {
                var drawingwithtag = new DrawingWithTag
                {
                    DateUtc = DateTime.UtcNow,
                    TagId = tagId
                };

                await drawingswithtagsTable.InsertAsync(drawingwithtag);

                //Synchronize drawings with tags
                await SyncDrawingsWithTags();
            }
        }

        public async Task SyncDrawingsWithTags()
        {
            //pull down all latest changes and then push current coffees up
            await drawingswithtagsTable.PullAsync("allDrawingsWithTags", drawingswithtagsTable.CreateQuery());
            await MobileService.SyncContext.PushAsync();
        }
    }
}