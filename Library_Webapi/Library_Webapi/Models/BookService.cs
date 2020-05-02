using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Library_Webapi.Service;
using Dapper;

namespace Library_Webapi.Models
{
	public class BookService : IBookService
	{
		//	connect string

		private string GetDBConnectionString()
		{
			return System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
		}

		// id exist
		public bool IsIdExist(string id)
		{
			bool result = false;
			string sql = @"SELECT bd.BOOK_ID AS ID
						FROM BOOK_DATA AS bd";
			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				var TempBookIdList = conn.Query<string>(sql);
				foreach (var BookId in TempBookIdList)
				{
					if (id == BookId)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		//dropdownlist and autocomplete
		public List<CustomSelectListItem> GetDropdownList(string option)
		{
			string sql = "";
			switch (option)
			{
				case "GetClassList":
					sql = @"SELECT bc.BOOK_CLASS_ID AS ID,bc.BOOK_CLASS_NAME AS NAME
						FROM BOOK_CLASS AS bc";
					break;
				case "GetKeeperList":
					sql = @"SELECT mm.[USER_ID] AS ID,mm.USER_ENAME AS NAME
						FROM MEMBER_M mm;";
					break;
				case "GetStatusList":
					sql = @"SELECT bc.CODE_ID AS ID ,bc.CODE_NAME AS NAME
						FROM BOOK_CODE bc	
						WHERE bc.CODE_TYPE = 'BOOK_STATUS'";
					break;
				case "GetUpdateKeeper":
					sql = @"SELECT mm.[USER_ID] AS ID,mm.USER_ENAME+'-'+mm.USER_CNAME AS NAME
						FROM MEMBER_M mm;";
					break;
			}
			List<CustomSelectListItem> selectList = new List<CustomSelectListItem>();
			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				var TempSelectList = conn.Query(sql);
				foreach (var List in TempSelectList)
				{
					selectList.Add(new CustomSelectListItem { Text = List.NAME, Value = List.ID });
				}
			}
			return selectList;
		}
		public List<string> GetBookName()
		{
			string sql = @"SELECT BOOK_NAME AS NAME
						FROM BOOK_DATA";
			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				List<string> bookNameList = conn.Query<string>(sql).ToList();
				return bookNameList;
			}
		}


		//search
		public List<BookSearch> SearchBook(BookSearch searchdata)
		{
			var sql = @"SELECT bd.BOOK_ID AS BookID,bc.BOOK_CLASS_NAME AS BookClassId,bd.BOOK_NAME AS BookName, CONVERT(varchar, bd.BOOK_BOUGHT_DATE, 111) AS BoughtDate, bcd.CODE_NAME AS BookStatusId, mm.USER_ENAME AS BookKeeperId
						FROM BOOK_DATA bd
								INNER JOIN BOOK_CLASS bc
									ON bd.BOOK_CLASS_ID = bc.BOOK_CLASS_ID
								INNER JOIN BOOK_CODE bcd
									ON bd.BOOK_STATUS = bcd.CODE_ID AND bcd.CODE_TYPE = 'BOOK_STATUS'
								LEFT OUTER JOIN MEMBER_M mm
									ON bd.BOOK_KEEPER = mm.[USER_ID]
						WHERE(bd.BOOK_CLASS_ID = @BookClassId OR @BookClassId = '') AND
								(LOWER(bd.BOOK_NAME) LIKE ('%'+LOWER(@BookName)+'%') OR @BookName = '') AND
								(bcd.CODE_ID = @BookStatusId OR @BookStatusId = '') AND
									(mm.[USER_ID] = @BookKeeperId OR @BookKeeperId = '')
						ORDER BY bd.CREATE_DATE DESC";

			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				var SearchList = conn.Query<Library_Webapi.Models.BookSearch>(sql, new
				{
					BookClassId = searchdata.BookClassId == null ? string.Empty : searchdata.BookClassId,
					BookName = searchdata.BookName == null ? string.Empty : searchdata.BookName,
					BookStatusId = searchdata.BookStatusId == null ? string.Empty : searchdata.BookStatusId,
					BookKeeperId = searchdata.BookKeeperId == null ? string.Empty : searchdata.BookKeeperId
				}).ToList();
				return SearchList;
			}
		}
		//insert
		public int InsertBook(BookInsert insertdata)
		{
			string sql = @"
						BEGIN TRY
							BEGIN TRANSACTION
								INSERT INTO BOOK_DATA
								 (
									BOOK_NAME,BOOK_CLASS_ID,BOOK_AUTHOR,
									BOOK_BOUGHT_DATE,BOOK_PUBLISHER,BOOK_NOTE,
									BOOK_STATUS,BOOK_KEEPER,BOOK_AMOUNT,CREATE_DATE,
									CREATE_USER,MODIFY_DATE,MODIFY_USER
								 )
								VALUES
								(
									 @BookName,@BookClass, @BookAuthor, 
									 @BoughtDate, @BookPublisher, @Note, 
									 'A','',0,GETDATE(), 
									 '123',GETDATE(), '123'
								)
							SELECT SCOPE_IDENTITY();
							COMMIT TRANSACTION
						End TRY
						BEGIN CATCH
							SELECT
								0 AS IsError
							ROLLBACK TRANSACTION	
						END CATCH ";

			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				int InsertID = conn.ExecuteScalar<int>(sql, new
				{
					BookName = insertdata.BookName,
					BookClass = insertdata.BookClass,
					BookAuthor = insertdata.BookAuthor,
					BoughtDate = insertdata.BoughtDate,
					BookPublisher = insertdata.BookPublisher,
					Note = insertdata.Note
				});
				return InsertID;
			}
		}

		//bookdatail
		public BookUpdate GetBookDetail(int id)
		{
			string sql = @"SELECT bd.BOOK_ID AS BookID,bd.BOOK_NAME AS BookName,bd.BOOK_AUTHOR AS BookAuthor,bd.BOOK_PUBLISHER AS BookPublisher,bd.BOOK_NOTE AS Note,
							CONVERT(varchar, bd.BOOK_BOUGHT_DATE, 111) AS BoughtDate,bc.BOOK_CLASS_NAME AS BookClassId,
							bcd.CODE_NAME AS BookStatusId,mm.USER_ENAME+'-'+mm.USER_CNAME AS BookKeeperId
						   FROM BOOK_DATA bd
								INNER JOIN  BOOK_CLASS bc
									ON bd.BOOK_CLASS_ID =bc.BOOK_CLASS_ID
								INNER JOIN BOOK_CODE bcd
									ON bd.BOOK_STATUS = bcd.CODE_ID AND bcd.CODE_TYPE = 'BOOK_STATUS'
								LEFT OUTER JOIN  MEMBER_M mm
									ON bd.BOOK_KEEPER = mm.[USER_ID]
							WHERE BOOK_ID = @BOOK_ID";
			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				var TempBookDetail = conn.Query<BookUpdate>(sql, new { BOOK_ID = id });
				BookUpdate BookDetail = new BookUpdate();
				foreach (var datail in TempBookDetail)
				{
					BookDetail = datail;
				}
				return BookDetail;
			}

		}

		//lendrecord
		public List<BookRecord> GetLendRecord(int BookId)
		{
			string sql = @" SELECT CONVERT(varchar,blr.LEND_DATE, 111) AS LendDate,blr.KEEPER_ID AS BookKeeperId,
							mm.USER_ENAME AS UserEname, mm.USER_CNAME AS UserCname
							FROM BOOK_LEND_RECORD AS blr
								INNER JOIN MEMBER_M AS mm
									ON blr.KEEPER_ID =mm.USER_ID
							WHERE blr.BOOK_ID=@BookId
							ORDER BY LendDate DESC";

			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				var LendRecordList = conn.Query<BookRecord>(sql, new { BookId = BookId }).ToList();
				return LendRecordList;
			}

		}
		//update
		public BookUpdate GetUpdateBook(int id)
		{
			string sql = @"SELECT bd.BOOK_NAME AS BookName,bd.BOOK_AUTHOR AS BookAuthor,bd.BOOK_PUBLISHER AS BookPublisher,bd.BOOK_NOTE AS Note,
							CONVERT(varchar, bd.BOOK_BOUGHT_DATE, 111) AS BoughtDate,bc.BOOK_CLASS_ID AS BookClassId,
							bcd.CODE_ID AS BookStatusId,mm.USER_ID AS BookKeeperId
						   FROM BOOK_DATA bd
								INNER JOIN  BOOK_CLASS bc
									ON bd.BOOK_CLASS_ID = bc.BOOK_CLASS_ID
								INNER JOIN BOOK_CODE bcd
									ON bd.BOOK_STATUS = bcd.CODE_ID AND bcd.CODE_TYPE = 'BOOK_STATUS'
								LEFT OUTER JOIN  MEMBER_M mm
									ON bd.BOOK_KEEPER = mm.[USER_ID]
							WHERE BOOK_ID = @BOOK_ID";
			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				var TempUpdateBook = conn.Query<BookUpdate>(sql, new { BOOK_ID = id });
				BookUpdate UpdateBook = new BookUpdate();
				foreach (var updateBook in TempUpdateBook)
				{
					UpdateBook = updateBook;
				}
				return UpdateBook;
			}
		}
		public void UpdateBook(BookUpdate updatedata)
		{
			string sql = @"
							BEGIN TRY
								BEGIN TRANSACTION		
									UPDATE BOOK_DATA 
									SET BOOK_NAME= @BOOK_NAME,
										BOOK_AUTHOR = @BOOK_AUTHOR,
										BOOK_PUBLISHER = @BOOK_PUBLISHER,
										BOOK_NOTE = @BOOK_NOTE,
										BOOK_BOUGHT_DATE = @BOOK_BOUGHT_DATE,
										BOOK_CLASS_ID = @BOOK_CLASS_ID,
										BOOK_STATUS = @BOOK_STATUS_ID,
										BOOK_KEEPER = @BOOK_KEEPER_ID,
										MODIFY_DATE = GETDATE()
									WHERE BOOK_ID = @BOOK_ID
								COMMIT TRANSACTION
							End TRY
							BEGIN CATCH
								SELECT
									ERROR_NUMBER() AS ErrorNumber,
									ERROR_SEVERITY() AS ErrorSeverity,
									ERROR_STATE() as ErrorState,
									ERROR_PROCEDURE() as ErrorProcedure,
									ERROR_LINE() as ErrorLine,
									ERROR_MESSAGE() as ErrorMessage
								ROLLBACK TRANSACTION
							END CATCH ";



			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				conn.Execute(sql, new
				{
					BOOK_NAME = updatedata.BookName,
					BOOK_AUTHOR = updatedata.BookAuthor,
					BOOK_PUBLISHER = updatedata.BookPublisher,
					BOOK_NOTE = updatedata.Note,
					BOOK_BOUGHT_DATE = updatedata.BoughtDate,
					BOOK_CLASS_ID = updatedata.BookClassId,
					BOOK_STATUS_ID = updatedata.BookStatusId,
					BOOK_KEEPER_ID = updatedata.BookKeeperId == null ? string.Empty : updatedata.BookKeeperId,
					BOOK_ID = updatedata.BookID
				});
			}

		}
		//delete
		public string DecideDelete(string BookID)
		{
			string sql = @"SELECT BOOK_STATUS 
							FROM BOOK_DATA
							WHERE BOOK_ID=@BookID";
			string StatusId = "";
			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				StatusId = conn.ExecuteScalar<string>(sql, new { BookID = BookID });
			}

			if (StatusId == "B" || StatusId == "C")
			{
				return "已借出不可刪除";
			}
			else
			{
				int id = Convert.ToInt32(BookID);
				this.DeleteBook(id);
				return "刪除成功";
			}
		}
		private void DeleteBook(int BookId)
		{
			string sql =
				@"
					 BEGIN TRY
						BEGIN TRANSACTION
							Delete FROM BOOK_DATA Where BOOK_ID=@BookId
						COMMIT TRANSACTION
					 End TRY
					 BEGIN CATCH
						SELECT
							ERROR_NUMBER() AS ErrorNumber,
							ERROR_SEVERITY() AS ErrorSeverity,
							ERROR_STATE() as ErrorState,
							ERROR_PROCEDURE() as ErrorProcedure,
							ERROR_LINE() as ErrorLine,
							ERROR_MESSAGE() as ErrorMessage
						ROLLBACK TRANSACTION
					 END CATCH ";
			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				conn.Execute(sql, new { BookId = BookId });
			}
		}


		/// <code>
		/// reader會動態綁定資料庫hand在那
		/// var reader = cmd.ExecuteReader(); 
		/// while (reader.Read()) 
		/// { selectlist.Add(new SelectListItem { Text = reader.GetString(1), Value = reader.GetString(val) }); }
		/// reader.Close();
		/// </code>
		/// <param name="insertdata"></param>
		//新增借閱紀錄到資料庫
		public void InsertLendRecord(LendRecordInsert insertdata)
		{
			string sql = @"
						BEGIN TRY
							BEGIN TRANSACTION
								INSERT INTO BOOK_LEND_RECORD
								 (
									BOOK_ID,KEEPER_ID,LEND_DATE,CRE_DATE,CRE_USR,MOD_DATE,MOD_USR
								 )
								VALUES
								(
									 @BookId,@KeeperId, GETDATE(), 
									 GETDATE(), '123', GETDATE(), 
									'123'
								)
							COMMIT TRANSACTION
						End TRY
						BEGIN CATCH
							SELECT
								ERROR_NUMBER() AS ErrorNumber,
								ERROR_SEVERITY() AS ErrorSeverity,
								ERROR_STATE() as ErrorState,
								ERROR_PROCEDURE() as ErrorProcedure,
								ERROR_LINE() as ErrorLine,
								ERROR_MESSAGE() as ErrorMessage
							ROLLBACK TRANSACTION	
						END CATCH ";

			using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
			{
				conn.Execute(sql, new
				{
					BookId = insertdata.BookID,
					KeeperId = insertdata.BookKeeperId
				});
			}

		}

	}

}