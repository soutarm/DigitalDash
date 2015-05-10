
namespace DigitalDash.Core.Classes
{
    class InAppPurchase
    {
        //async void LoadProductListingsByProductIds()
        //{
        //    // First, retrieve the list of some products by their IDs.
        //    ListingInformation listings = await CurrentApp.LoadListingInformationByProductIdsAsync(new string[] { "Bag of 50 gold", "Bag of 100 gold" });

        //    // Then, use the flat list of products as the data source for a
        //    // list box containing data-bound list items.
        //    ProductListBox.ItemsSource = listings.ProductListings.Values;
        //}

        //async void PurchaseProduct(string productId)
        //{
        //    try
        //    {
        //        // Kick off purchase; don't ask for a receipt when it returns
        //        await CurrentApp.RequestProductPurchaseAsync(productId, false);

        //        // Now that purchase is done, give the user the goods they paid for
        //        // (DoFulfillment is defined later)
        //        DoFulfillment();
        //    }
        //    catch (Exception ex)
        //    {
        //        // When the user does not complete the purchase (e.g. cancels or navigates back from the Purchase Page), an exception with an HRESULT of E_FAIL is expected.
        //    }
        //}

        //// This should be replaced in your code with a persistent storage mechanism that is tamper-resistant.
        //int m_goldCount = 0;
        //int m_silverCount = 0;

        ////
        //// Fulfillment of consumable in-app products
        //public void DoFulfillment()
        //{
        //    var productLicenses = CurrentApp.LicenseInformation.ProductLicenses;

        //    // Check fulfillment for consumable products with hard-coded asset counts
        //    MaybeGiveMeGold(productLicenses["Bag of 50 gold"], 50);
        //    MaybeGiveMeGold(productLicenses["Bag of 100 gold"], 100);

        //    // Check fulfillment for consumable products with variable asset counts
        //    MaybeGiveMeSilver(productLicenses);
        //}

        //// Count is passed in as a parameter
        //void MaybeGiveMeGold(ProductLicense license, int goldCount)
        //{
        //    if (license.IsConsumable && license.IsActive)
        //    {
        //        m_goldCount += goldCount;
        //        CurrentApp.ReportProductFulfillment(license.ProductId);
        //    }
        //}

        //// Count is part of the product ID
        //void MaybeGiveMeSilver(IReadOnlyDictionary<string, ProductLicense> productLicenses)
        //{
        //    Regex bagOfSilver = new Regex(@"Bag\.Silver\.(\d+)");

        //    foreach (ProductLicense license in productLicenses.Values)
        //    {
        //        if (license.IsConsumable && license.IsActive)
        //        {
        //            MatchCollection m = bagOfSilver.Matches(license.ProductId);

        //            if ((m.Count == 2) && (m[1].Success))
        //            {
        //                m_silverCount += int.Parse(m[1].Value);
        //                CurrentApp.ReportProductFulfillment(license.ProductId);
        //            }
        //        }
        //    }
        //}

        //async Task<bool> LoadLevelAsync(string levelProductId)
        //{
        //    ProductLicense license = CurrentApp.LicenseInformation.ProductLicenses[levelProductId];

        //    if (!license.IsActive)
        //    {
        //        // User doesn't own this level
        //        return false;
        //    }

        //    if (!IsLevelDownloaded(levelProductId))
        //    {
        //        string receiptXml = await CurrentApp.GetProductReceiptAsync(levelProductId);

        //        await DownloadLevelAsync(receiptXml);
        //    }

        //    // TODO: Load the level
        //    return true;
        //}

        //async Task DownloadLevelAsync(string receiptXml)
        //{
        //    //var webReq = (HttpWebRequest)WebRequest.Create(sc_DownloadUrl);

        //    //webReq.Method = "POST";

        //    //AddStringToWebRequestStream(webReq, receiptXml);

        //    //WebResponse response = await webReq.GetResponseAsync();

        //    // TODO: Save the level to disk
        //}


    }
}
