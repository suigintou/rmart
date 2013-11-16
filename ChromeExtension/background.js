function uploadFromUrl(tab, url) {
	var reqUrl =
		"http://rmart.org/Upload/FromUrl" +
		"?url=" + escape(url) +
		"&referer=" + escape(tab.url);

	var isPixiv = false;
	if(tab.url.match(/^http:\/\/www.pixiv.net\/member_illust.php\?mode=big&illust_id=\d+$/i)) {
		reqUrl += "&replaceFile=True&source=" + escape(tab.url.replace("mode=big", "mode=medium"));
		isPixiv = true;
	}
	else if(tab.url.match(/^http:\/\/www.pixiv.net\/member_illust.php\?mode=manga_big&illust_id=\d+\&page=\d+$/i)) {
		reqUrl += "&replaceFile=True&source=" + escape(tab.url.replace("mode=manga_big", "mode=medium").replace(/\&page=\d+/i, ""));
		isPixiv = true;
	}
	
	if(isPixiv) {
		reqUrl += "&tags=" + escape("artist:" + /\/img\/([^\/]+)/g.exec(url)[1]);
	}

	chrome.tabs.update(tab.id, { "url" : reqUrl });
}

chrome.contextMenus.create({
	"title" : "Upload to rmart.org",
	"contexts" : ["image"],
	"onclick" : function(info, tab) { uploadFromUrl(tab, info.srcUrl) }
})