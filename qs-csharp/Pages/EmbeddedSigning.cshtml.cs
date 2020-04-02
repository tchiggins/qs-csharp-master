using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;


namespace qs_csharp.Pages
{
	public class EmbeddedSigning : PageModel
	{
		// Constants need to be set:
		private const string accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjY4MTg1ZmYxLTRlNTEtNGNlOS1hZjFjLTY4OTgxMjIwMzMxNyJ9.eyJUb2tlblR5cGUiOjUsIklzc3VlSW5zdGFudCI6MTU4NTg0ODA2NiwiZXhwIjoxNTg1ODc2ODY2LCJVc2VySWQiOiI5ZThjZDcwMS1iZjI4LTQ2ZTgtOTkwZi1jOGZiZmM0MTg3MDQiLCJzaXRlaWQiOjEsInNjcCI6WyJzaWduYXR1cmUiLCJjbGljay5tYW5hZ2UiLCJvcmdhbml6YXRpb25fcmVhZCIsInJvb21fZm9ybXMiLCJncm91cF9yZWFkIiwicGVybWlzc2lvbl9yZWFkIiwidXNlcl9yZWFkIiwidXNlcl93cml0ZSIsImFjY291bnRfcmVhZCIsImRvbWFpbl9yZWFkIiwiaWRlbnRpdHlfcHJvdmlkZXJfcmVhZCIsImR0ci5yb29tcy5yZWFkIiwiZHRyLnJvb21zLndyaXRlIiwiZHRyLmRvY3VtZW50cy5yZWFkIiwiZHRyLmRvY3VtZW50cy53cml0ZSIsImR0ci5wcm9maWxlLnJlYWQiLCJkdHIucHJvZmlsZS53cml0ZSIsImR0ci5jb21wYW55LnJlYWQiLCJkdHIuY29tcGFueS53cml0ZSJdLCJhdWQiOiJmMGYyN2YwZS04NTdkLTRhNzEtYTRkYS0zMmNlY2FlM2E5NzgiLCJhenAiOiJmMGYyN2YwZS04NTdkLTRhNzEtYTRkYS0zMmNlY2FlM2E5NzgiLCJpc3MiOiJodHRwczovL2FjY291bnQtZC5kb2N1c2lnbi5jb20vIiwic3ViIjoiOWU4Y2Q3MDEtYmYyOC00NmU4LTk5MGYtYzhmYmZjNDE4NzA0IiwiYW1yIjpbImludGVyYWN0aXZlIl0sImF1dGhfdGltZSI6MTU4NTg0ODA2MywicHdpZCI6ImIzMDZlYjAwLWQ3NjUtNGUzZi1iMWIzLTQ2ZDQ5MmZiZGM2MCJ9.V3Pux3S0K07D-1XAjCJnU-YCHKmNzFSfxZAo-ws91QhJTM4gyZPZygkXh2Aje65-NBkODI4Ogg5XjrqBpV6EcbQZYXcRz9p1ejzvaT9k1-0rsIZAzq8XHrBYW7uY4OGCpoKp1FWyjEfq6cII1uFFC7z8s2Yn0-jyMnro4WsEXM5iwv39R5jVZCVIQJhF1vRERN1Mwk5UpLpVfcsxVXVRQPKiT3jF-PLefMPsGhtZZnKaPJBwGDw5yHaxcA0vW7CIvKD_2zwPrT_vEUhyFysiocJU9hdu5_r2K0uIJguNZgl9xAoDmDlGStm4Q-uHJTsJK-TUgqCzBR2NitRRvRGM8A";
		private const string accountId = "10261958";
		private const string signerName = "Tom Higgins";
		private const string signerEmail = "tom@gold-i.com";
		private const string docName = "World_Wide_Corp_lorem.pdf";

		// Additional constants
		private const string signerClientId = "1000";
		private const string basePath = "https://demo.docusign.net/restapi";

		// Change the port number in the Properties / launchSettings.json file:
		//     "iisExpress": {
		//        "applicationUrl": "http://localhost:5050",
		private const string returnUrl = "http://localhost:5050/DSReturn";

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			// Embedded Signing Ceremony
			// 1. Create envelope request obj
			// 2. Use the SDK to create and send the envelope
			// 3. Create Envelope Recipient View request obj
			// 4. Use the SDK to obtain a Recipient View URL
			// 5. Redirect the user's browser to the URL

			// 1. Create envelope request object
			//    Start with the different components of the request
			//    Create the document object
			Document document = new Document
			{
				DocumentBase64 = Convert.ToBase64String(ReadContent(docName)),
				Name = "Lorem Ipsum",
				FileExtension = "pdf",
				DocumentId = "1"
			};
			Document[] documents = new Document[] { document };

			// Create the signer recipient object 
			Signer signer = new Signer
			{
				Email = signerEmail,
				Name = signerName,
				ClientUserId = signerClientId,
				RecipientId = "1",
				RoutingOrder = "1"
			};

			// Create the sign here tab (signing field on the document)
			SignHere signHereTab = new SignHere
			{
				DocumentId = "1",
				PageNumber = "1",
				RecipientId = "1",
				TabLabel = "Sign Here Tab",
				XPosition = "195",
				YPosition = "147"
			};
			SignHere[] signHereTabs = new SignHere[] { signHereTab };

			// Add the sign here tab array to the signer object.
			signer.Tabs = new Tabs { SignHereTabs = new List<SignHere>(signHereTabs) };
			// Create array of signer objects
			Signer[] signers = new Signer[] { signer };
			// Create recipients object
			Recipients recipients = new Recipients { Signers = new List<Signer>(signers) };
			// Bring the objects together in the EnvelopeDefinition
			EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
			{
				EmailSubject = "Please sign the document",
				Documents = new List<Document>(documents),
				Recipients = recipients,
				Status = "sent"
			};

			// 2. Use the SDK to create and send the envelope
			ApiClient apiClient = new ApiClient(basePath);
			apiClient.Configuration.AddDefaultHeader("Authorization", "Bearer " + accessToken);
			EnvelopesApi envelopesApi = new EnvelopesApi(apiClient.Configuration);
			EnvelopeSummary results = await envelopesApi.CreateEnvelopeAsync(accountId, envelopeDefinition);

			// 3. Create Envelope Recipient View request obj
			string envelopeId = results.EnvelopeId;
			RecipientViewRequest viewOptions = new RecipientViewRequest
			{
				ReturnUrl = returnUrl,
				ClientUserId = signerClientId,
				AuthenticationMethod = "none",
				UserName = signerName,
				Email = signerEmail
			};

			// 4. Use the SDK to obtain a Recipient View URL
			ViewUrl viewUrl = await envelopesApi.CreateRecipientViewAsync(accountId, envelopeId, viewOptions);

			// 5. Redirect the user's browser to the URL
			return Redirect(viewUrl.Url);
		}

		/// <summary>
		/// This method read bytes content from the project's Resources directory
		/// </summary>
		/// <param name="fileName">resource path</param>
		/// <returns>return bytes array</returns>
		internal static byte[] ReadContent(string fileName)
		{
			byte[] buff = null;
			string path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", fileName);
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(stream))
				{
					long numBytes = new FileInfo(path).Length;
					buff = br.ReadBytes((int)numBytes);
				}
			}
			return buff;
		}
	}
}
