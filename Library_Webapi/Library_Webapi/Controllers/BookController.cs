using Library_Webapi.Models;
using Library_Webapi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Library_Webapi.Controllers
{
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class BookController : ApiController
	{

		private IBookService _bookservice;
		public BookController(IBookService bookService)
		{
			_bookservice = bookService;
		}

		public IHttpActionResult GetDropdownList_Json(string type)
		{
			List<CustomSelectListItem> BookSelectList = _bookservice.GetDropdownList(type);
			return Ok(BookSelectList);
		}
		public IHttpActionResult GetBookName()
		{
			List<string> BookName = _bookservice.GetBookName();
			return Ok(BookName);

		}

		public IHttpActionResult GetBooksGrid([FromUri] BookSearch booksearch)
		{
			List<BookSearch> Grid = _bookservice.SearchBook(booksearch);
			return Ok(Grid);
		}

		/*---------------------BookDetail---------------------*/
		public IHttpActionResult GetBookDetail(string id)
		{
			if (_bookservice.IsIdExist(id))
			{
				BookUpdate bookdetail = _bookservice.GetBookDetail(Convert.ToInt32(id));
				return Ok(bookdetail);
			}
			else return NotFound();
		}

		/*---------------------Insert---------------------*/
		[HttpPost]
		public void InsertBook(BookInsert insertdata)
		{
			int InsertID = _bookservice.InsertBook(insertdata); //test用
		}

		/*---------------------LENDRECORD---------------------*/
		public IHttpActionResult GetLendRecord(string id)
		{

			if (_bookservice.IsIdExist(id))
			{
				List<BookRecord> bookRecord = _bookservice.GetLendRecord(Convert.ToInt32(id));
				return Ok(bookRecord);
			}
			else return NotFound();
		}

		/*---------------------DELETE---------------------*/
		[HttpPost()]
		public IHttpActionResult DeleteBook([FromBody]string BookID)
		{
			return Ok(_bookservice.DecideDelete(BookID));
		}

		/*---------------------UPDATE---------------------*/

		public IHttpActionResult GetUpdateData(string id)
		{
			if (_bookservice.IsIdExist(id))
			{
				BookUpdate bookupdate = _bookservice.GetUpdateBook(Convert.ToInt32(id));
				return Ok(bookupdate);
			}
			else return NotFound();
		}

		[HttpPost()]
		public IHttpActionResult UpdateBook(BookUpdate bookUpdate, [FromUri]string IniBookStatus,[FromUri] string LaterBookStatus)
		{
			//借閱人借閱狀態關係
			if ((bookUpdate.BookStatusId == "B" || bookUpdate.BookStatusId == "C") && (bookUpdate.BookKeeperId == "" || bookUpdate.BookKeeperId == null))
			{
				return Ok("此借閱狀態,借閱人不能為空");
			}
			else
			{
				//insert 借閱紀錄 
				if ((IniBookStatus == "A" || IniBookStatus == "U") && (LaterBookStatus == "B" || LaterBookStatus == "C"))
				{
					LendRecordInsert lendRecordInsert = new LendRecordInsert();
					lendRecordInsert.BookKeeperId = bookUpdate.BookKeeperId;
					lendRecordInsert.BookID = Convert.ToInt32(bookUpdate.BookID);

					_bookservice.InsertLendRecord(lendRecordInsert);

				}
				//update 書本資料
				_bookservice.UpdateBook(bookUpdate);
				return Ok("編輯成功");
			}
		}
	}

}
