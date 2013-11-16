var contextMenu = require("sdk/context-menu");
var tabs = require("sdk/tabs");

contextMenu.Item({
	label: "Upload to rmart.org",
	context: contextMenu.SelectorContext("img"),
	contentScript: 'self.on("click", function (node, data) {' +
	               '	self.postMessage({ imgSrc: node.src, referer: document.URL });' +
	               '});',
	onMessage: function (context) {
		uploadFromUrl(context.imgSrc, context.referer)
	}
});

function uploadFromUrl(imgSrc, referer) {
	var reqUrl =
		"http://rmart.org/Upload/FromUrl" +
		"?url=" + escape(imgSrc) +
		"&referer=" + escape(referer);

	var isPixiv = false;
	if(referer.match(/^http:\/\/www.pixiv.net\/member_illust.php\?mode=big&illust_id=\d+$/i)) {
		reqUrl += "&replaceFile=True&source=" + escape(referer.replace("mode=big", "mode=medium"));
		isPixiv = true;
	}
	else if(referer.match(/^http:\/\/www.pixiv.net\/member_illust.php\?mode=manga_big&illust_id=\d+\&page=\d+$/i)) {
		reqUrl += "&replaceFile=True&source=" + escape(referer.replace("mode=manga_big", "mode=medium").replace(/\&page=\d+/i, ""));
		isPixiv = true;
	}
		
	if(isPixiv) {
		reqUrl += "&tags=" + escape("artist:" + /\/img\/([^\/]+)/g.exec(imgSrc)[1]);
	}

	tabs.activeTab.url = reqUrl;
}