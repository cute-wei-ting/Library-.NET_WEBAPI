var ServerPreaddress = "https://localhost:44332/api";

$(document).ready(function () {

	var Today = new Date();
	$(".header").load("./header.html");
	$("footer > p").append(Today.getFullYear() + "- My ASP.NET Application");

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

	//Insert
	var validator = $("#InsertForm").kendoValidator().data("kendoValidator");
	$("#insert-btn").click(function (e) {
		if (validator.validate()) {
			$.ajax({
				type: "POST",
				url: ServerPreaddress + "/Book/InsertBook",
				data: JSON.stringify({
					BookName: $("#BookName").val(),
					BookAuthor: $("#BookAuthor").val(),
					BookPublisher: $("#BookPublisher").val(),
					Note: $("#Note").val(),
					BoughtDate: $("#BoughtDate").val(),
					BookClass: $("#BookClassId").data("kendoDropDownList").value()
				}),
				contentType: "application/json",
				//dataType:"json",
				//json回傳void會出錯
				success: function (response) {
					alert("新增成功");
					var address = "Index.html";
					location.replace(address);
				}, error: function (error) {
					alert("新增失敗");
				}
			});
		}
		else {
			console.log("insert validation error");
		}
	});
});