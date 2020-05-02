using Library_Webapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Webapi.Service
{
	public interface IBookService
	{
		bool IsIdExist(string id);
		List<CustomSelectListItem> GetDropdownList(string option);
		List<string> GetBookName();

		string DecideDelete(string BookID);
		BookUpdate GetBookDetail(int id);
		List<BookRecord> GetLendRecord(int BookId);
		BookUpdate GetUpdateBook(int id);
		int InsertBook(BookInsert insertdata);
		void InsertLendRecord(LendRecordInsert insertdata);
		List<BookSearch> SearchBook(BookSearch searchdata);
		void UpdateBook(BookUpdate updatedata);
	}
}
