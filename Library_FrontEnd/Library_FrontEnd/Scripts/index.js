var ServerPreaddress = "https://localhost:44332/api";

$(document).ready(function () {
	var Today = new Date();
	$(".header").load("./header.html");
	$("footer > p").append(Today.getFullYear() + "- My ASP.NET Application");

	//AutoComplete
	$("#BookName").kendoAutoComplete({
		dataSource: {
			transport: {
				read: {
					type: "GET",
					url: ServerPreaddress+"/Book/GetBookName",
					dataType: "json"
				}
			}
		},
		filter: "startswith"
	});
	//DropDownList
	$("#BookClassId").kendoDropDownList({
		dataTextField: "Text",
		dataValueField: "Value",
		optionLabel: "請選擇",
		dataSource: {
			transport: {
				read: {
					type: "GET",
					url: ServerPreaddress +"/Book/GetDropdownList_Json",
					dataType: "json",
					data: { type: "GetClassList" }
				}
			}
		}
	});
	$("#BookKeeperId").kendoDropDownList({
		dataTextField: "Text",
		dataValueField: "Value",
		optionLabel: "請選擇",
		dataSource: {
			transport: {
				read: {
					type: "GET",
					url: ServerPreaddress +"/Book/GetDropdownList_Json",
					dataType: "json",
					data: { type: "GetKeeperList" }
				}
			}
		}
	});
	$("#BookStatusId").kendoDropDownList({
		dataTextField: "Text",
		dataValueField: "Value",
		optionLabel: "請選擇",
		dataSource: {
			transport: {
				read: {
					type: "GET",
					url: ServerPreaddress +"/Book/GetDropdownList_Json",
					dataType: "json",
					data: { type: "GetStatusList" }
				}
			}
		}
	});
	//Grid
	$("#tbcontainer").kendoGrid({
		dataSource: {
			transport: {
				read: {
					url: ServerPreaddress + "/Book/GetBooksGrid", dataType: "json"
				}
			},
			schema: {
				model: {
					fields: {
						BookID: { type: "string" },
						BookClassId: { type: "string" },
						BookName: { type: "string" },
						BoughtDate: { type: "string" },
						BookStatusId: { type: "string" },
						BookStatus: { type: "string" },
						BookKeeperId: { type: "string" }
					}
				}
			},
			pageSize: 20,
		},
		height: 550,
		sortable: true,
		pageable: {
			input: true,
			numeric: true
		},
		editable: false,//edit開了功能全開
		columns: [
			{ hidden: true, field: "BookID" },
			{ field: "BookClassId", title: "圖書類別", width: "30%", headerAttributes: { style: "text-align: center" } },
			{ field: "BookName", title: "書名", width: "40%", template: "<a href=\"javascript:void(0);\" onclick=\"BookDetail()\">#: BookName # </a>", headerAttributes: { style: "text-align: center" } },
			{ field: "BoughtDate", title: "購書日期", width: "20%", headerAttributes: { style: "text-align: center" } },
			{ field: "BookStatusId", title: "借閱狀態", width: "20%", headerAttributes: { style: "text-align: center" } },
			{ field: "BookKeeperId", title: "借閱人", width: "15%", headerAttributes: { style: "text-align: center" } },
			{ command: { text: "借閱紀錄", click: lendRecord }, title: "", width: "120px", headerAttributes: { style: "text-align: center" } },
			{ command: { text: "編輯", click: updateBook }, title: "", width: "100px", headerAttributes: { style: "text-align: center" } },
			{ command: { text: "刪除", click: deleteBook }, title: "", width: "100px", headerAttributes: { style: "text-align: center" } }
		]

	});

	//Search
	$("#search-btn").click(function (e) {
		$.ajax({
			type: "GET",
			url: ServerPreaddress +"/Book/GetBooksGrid",
			data: {
				BookName: $("#BookName").val(),
				BookClassId: $("#BookClassId").data("kendoDropDownList").value(),//kendoDropDownList 避免ajax發送順序問題
				BookKeeperId: $("#BookKeeperId").data("kendoDropDownList").value(),
				BookStatusId: $("#BookStatusId").data("kendoDropDownList").value()
			},
			success: function (data) {
				var dataSource = new kendo.data.DataSource({
					data: data
				});
				$("#tbcontainer").data("kendoGrid").setDataSource(dataSource);
			}

		});
	});

	//Clear
	$("#clear-btn").click(function (e) {
		$("#BookName").val("");
		$("#BookClassId").data("kendoDropDownList").value("");
		$("#BookKeeperId").data("kendoDropDownList").value("");
		$("#BookKeeperId").data("kendoDropDownList").enable(true);
		$("#BookStatusId").data("kendoDropDownList").value("");
	});


	//借閱人跟借閱狀態關係
	bookstatus();
	$("#BookStatusId").change(bookstatus);
}); 


/*FUNCTION*/
//LendRecord
function lendRecord(e) {
	e.preventDefault();
	var tr = event.target.closest("tr");
	var grid = $("#tbcontainer").data("kendoGrid");
	var dataItem = grid.dataItem(tr);
	var address = "LendRecord.html?BookID=" + dataItem.BookID;
	location.href=address;
}

//Update
function updateBook(e) {
	e.preventDefault();
	var tr = event.target.closest("tr");
	var grid = $("#tbcontainer").data("kendoGrid");
	var dataItem = grid.dataItem(tr);
	var address = "UpdateBook.html?BookID=" + dataItem.BookID;
	location.href = address;
}	

//Delete
function deleteBook(e) {
	e.preventDefault();
	var tr = e.target.closest("tr");
	var grid = $("#tbcontainer").data("kendoGrid");
	var dataItem = grid.dataItem(tr);

	if (confirm("是否刪除")) {
		$.ajax({
			type: "POST",
			url: ServerPreaddress+"/Book/DeleteBook",
			//data: { "": dataItem.BookID },
			data: dataItem.BookID,
			contentType: "application/json",
			dataType:"text",
			success: function (response) {
				$("#tbcontainer").data("kendoGrid").dataSource.read();
				alert(response);
			}, error: function (error) {
				alert("系統發生錯誤");
			}
		});
	}
}


//書本明細
function BookDetail() {
	var tr = event.target.closest("tr");
	var grid = $("#tbcontainer").data("kendoGrid");
	var dataItem = grid.dataItem(tr);
	var address = "BookDetail.html?BookID=" + dataItem.BookID;
	location.href = address;
}

function bookstatus() {
	if ($("#BookStatusId").data("kendoDropDownList").value() == "A" || $("#BookStatusId").data("kendoDropDownList").value() == "U") {
		$("#BookKeeperId").data("kendoDropDownList").value("");
		$("#BookKeeperId").data("kendoDropDownList").enable(false);
	}
	else {
		$("#BookKeeperId").data("kendoDropDownList").enable(true);
	}
}