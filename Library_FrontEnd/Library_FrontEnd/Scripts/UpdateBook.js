var ServerPreaddress = "https://localhost:44332/api";

$(document).ready(function () {

	var Today = new Date();
	$(".header").load("./header.html");
	$("footer > p").append(Today.getFullYear() + "- My ASP.NET Application");

	var IniBookStatus = "";
	var LaterBookStatus = "";
	var id = oneValues();
	$("#BoughtDate").kendoDatePicker({
		format: "yyyy/MM/dd"
	});
	$("#BookClassId").kendoDropDownList({
		dataTextField: "Text",
		dataValueField: "Value",
		optionLabel: "請選擇",
		dataSource: {
			transport: {
				read: {
					type: "GET",
					url: ServerPreaddress+"/Book/GetDropdownList_Json",
					dataType: "json",
					data: { type: "GetClassList" }
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
					url: ServerPreaddress+"/Book/GetDropdownList_Json",
					dataType: "json",
					data: { type: "GetStatusList" }
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
					url: ServerPreaddress+"/Book/GetDropdownList_Json",
					dataType: "json",
					data: { type: "GetUpdateKeeper" }
				}
			}
		}
	});

	//GetUpdateData
	$.ajax({
		type: "GET",
		url: ServerPreaddress+"/Book/GetUpdateData/"+id,
		success: function (response) {
			$("#BookName").val(response["BookName"]);//val()是function
			$("#BookAuthor").val(response["BookAuthor"]);
			$("#BookPublisher").val(response["BookPublisher"]);
			$("#Note").val(response["Note"]);
			$("#BoughtDate").val(response["BoughtDate"]);
			$("#BookClassId").data("kendoDropDownList").value(response["BookClassId"]); //會等dropdownlist建好
			$("#BookStatusId").data("kendoDropDownList").value(response["BookStatusId"]);
			$("#BookKeeperId").data("kendoDropDownList").value(response["BookKeeperId"]);
			bookstatus();//借閱人跟借閱狀態關係
			IniBookStatus = $("#BookStatusId").data("kendoDropDownList").value();//拿到一開始狀態,判斷是否增加借閱紀錄
		}, error: function (error) {
		}
	});

	//Sav
	var validator = $("#UpdateForm").kendoValidator().data("kendoValidator")
	$("#UpdateSave").click(function () {
		if (validator.validate()) {
			LaterBookStatus = $("#BookStatusId").data("kendoDropDownList").value();//拿到之後狀態
			//跟Update至 BOOK_DATA
			$.ajax({
				type: "POST",
				url: ServerPreaddress + "/Book/UpdateBook?IniBookStatus=" + IniBookStatus + "&LaterBookStatus=" + LaterBookStatus,
				data: JSON.stringify({
					BookID:id,
					BookName: $("#BookName").val(),
					BookAuthor: $("#BookAuthor").val(),
					BookPublisher: $("#BookPublisher").val(),
					Note: $("#Note").val(),
					BoughtDate: $("#BoughtDate").val(),
					BookClassId: $("#BookClassId").data("kendoDropDownList").value(),
					BookStatusId: $("#BookStatusId").data("kendoDropDownList").value(),
					BookKeeperId: $("#BookKeeperId").data("kendoDropDownList").value()
				}),
				contentType: "application/json",
				dataType: "text",//純文字
				success: function (data) {
					alert(data);
				}, error: function (error) {
					alert("系統發生錯誤");
				}
			});
		}
		else {
			console.log("Update validation error");
		}
	});

	//Del
	$("#UpdateDel").click(function () {
		if (confirm("是否刪除")) {
			$.ajax({
				type: "POST",
				url: ServerPreaddress + "/Book/DeleteBook",
				data: id,
				contentType: "application/json",
				dataType: "text",
				success: function (response) {
					alert(response);
					var address = "Index.html";
					location.replace(address);
				}, error: function (error) {
					alert("系統發生錯誤");
				}
			});
		}
	});//data:{"":id}   contentType:www-urlform

	$("#BookStatusId").change(bookstatus);
});


/*FUNCTION*/
//借閱人跟借閱狀態關係
function bookstatus() {

	if ($("#BookStatusId").data("kendoDropDownList").value() == "A" || $("#BookStatusId").data("kendoDropDownList").value() == "U") {
		$("#BookKeeperId").data("kendoDropDownList").value("");
		$("#BookKeeperId").data("kendoDropDownList").enable(false);
	}
	else {
		$("#BookKeeperId").data("kendoDropDownList").enable(true);
	}
}
function oneValues() {
	var result;
	var url = location.search;
	if (url.indexOf("?") != -1) {
		result = url.substr(url.indexOf("=") + 1);
	}
	return result;
}