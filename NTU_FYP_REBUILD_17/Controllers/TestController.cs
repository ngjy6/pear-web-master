using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace NTU_FYP_REBUILD_17.Controllers
{
	public class TestController : ApiController
    {

		[HttpGet]
		[Route("api/Test/TestGet")]
		public string TestGet()
		{
			return "Get method works.";
		}

		[HttpPost]
		[Route("api/Test/TestPost")]
		public string TestPost()
		{
			return "Post method works.";
		}

		[HttpPut]
		[Route("api/Test/TestPut")]
		public string TestPut()
		{
			return "Put method works.";
		}

		[HttpDelete]
		[Route("api/Test/TestDelete")]
		public string TestDelete()
		{
			return "Delete method works.";
		}

		[HttpGet]
		[Route("api/Test/testGetParameter")]
		public string testGetParameter(string input)
		{
			return "Get request type. Message:" + input;
		}

		[HttpPost]
		[Route("api/Test/testPostParameter")]
		public string testPostParameter(string input)
		{
			return "Post request type. Message:" + input;
		}

		[HttpPut]
		[Route("api/Test/testPutParameter")]
		public string testPutParameter(string input)
		{
			return "Put request type. Message:" + input;
		}

		[HttpDelete]
		[Route("api/Test/testDeleteParameter")]
		public string testDeleteParameter(string input)
		{
			string x = "1234";// test
			System.Diagnostics.Debug.WriteLine(x);
			return "Delete request type. Message:" + input;
		}

	}
}
