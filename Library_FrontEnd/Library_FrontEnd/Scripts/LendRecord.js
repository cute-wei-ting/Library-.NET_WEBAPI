var ServerPreaddress = "https://localhost:44332/api";
$(document).ready(function () {

	var Today = new Date();
	$(".header").load("./header.html");
	$("footer > p").append(Today.getFullYear() + "- My ASP.NET Application");

	var id = oneValues();
	$("#recordtbcontainer").kendoGrid({
		dataSource: {
			transport: {
				read: {
					type: "GET", url: ServerPreaddress +"/Book/GetLendRecord/"+id, dataType: "json"
				}
			},
			schema: {
				model: {
					fields: {
						LendDate: { type: "string" },
						BookKeeperId: { type: "string" },
						UserEname: { type: "string" },
						UserCname: { type: "string" }
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
			{ field: "LendDate", title: "借閱日期", width: "30%", headerAttributes: { style: "text-align: center" } },
			{ field: "BookKeeperId", title: "借閱人員編號", width: "15%", headerAttributes: { style: "text-align: center" } },
			{ field: "UserEname", title: "英文姓名", width: "30%", headerAttributes: { style: "text-align: center" } },
			{ field: "UserCname", title: "中文姓名", width: "20%", headerAttributes: { style: "text-align: center" } },
		],
	});
});
function oneValues() {
	var result;
	var url = location.search;
	if (url.indexOf("?") != -1) {
		result = url.substr(url.indexOf("=") + 1);
	}
	return result;
}