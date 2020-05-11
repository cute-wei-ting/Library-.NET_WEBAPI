var ServerPreaddress = "https://localhost:44332/api";

$(document).ready(function () {
	var Today = new Date();
	$(".header").load("./header.html");
	$("footer > p").append(Today.getFullYear() + "- My ASP.NET Application");

	    var id = oneValues();
		$("#detailtbcontainer").kendoGrid({
			dataSource: {
				transport: {
					read: {
						type: "GET", url: ServerPreaddress + "/Book/GetBookDetail/"+ id, dataType: "json"
					}
				},
				schema: {
					model: {
						fields: {
							BookID: { type: "string" },
							BookName: { type: "string" },
							BookAuthor: { type: "string" },
							BookPublisher: { type: "string" },
							Note: { type: "string" },
							BoughtDate: { type: "string" },
							BookClassId: { type: "string" },
							BookStatusId: { type: "string" },
							BookKeeperId: { type: "string" }
						}
					}
				},
				pageSize: 20,
			},
			height: 200,
			sortable: false,
			pageable: {
				input: false,
				numeric: false
			},
			editable: false,//edit開了功能全開
			columns: [
				{ hidden: true, field: "BookID" },
				{ field: "BookName", title: "書名", width: "30%", headerAttributes: { style: "text-align: center" } },
				{ field: "BookAuthor", title: "作者", width: "15%", headerAttributes: { style: "text-align: center" } },
				{ field: "BookPublisher", title: "出版商", width: "30%", headerAttributes: { style: "text-align: center" } },
				{ field: "Note", title: "內容簡介", width: "20%", headerAttributes: { style: "text-align: center" } },
				{ field: "BoughtDate", title: "購書日期", width: "25%", headerAttributes: { style: "text-align: center" } },
				{ field: "BookClassId", title: "圖書類別", width: "35%", headerAttributes: { style: "text-align: center" } },
				{ field: "BookStatusId", title: "借閱狀態", width: "20%", headerAttributes: { style: "text-align: center" } },
				{ field: "BookKeeperId", title: "借閱人", width: "25%", headerAttributes: { style: "text-align: center" } },
				{ command: { text: "編輯", click: ModifyBook }, title: "", width: "100px", headerAttributes: { style: "text-align: center" } }
			],
		});
});

/*FUNCTION*/
function ModifyBook() {
	var tr = event.target.closest("tr");
	var grid = $("#detailtbcontainer").data("kendoGrid");
	var dataItem = grid.dataItem(tr);
	var address = "UpdateBook.html?BookID=" + dataItem.BookID;
	location.href=address;
}
function oneValues() {
	var result;
	var url = location.search;
	if (url.indexOf("?") != -1) {
		result = url.substr(url.indexOf("=") + 1);
	}
	return result;
}
