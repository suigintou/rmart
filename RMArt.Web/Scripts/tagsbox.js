(function () {

	$(function () {

		$(".tags-box").typeahead({
			source: function (query, process) {
				$.getJSON("/Tags/AutoCompletionList", { term: extractLast(query) }, process);
			},
			updater: function (item) {
				var terms = split(this.query);
				terms.pop();
				terms.push(item);
				terms.push("");
				return terms.join(", ");
			},
			matcher: function (item) {
				return extractLast(this.query).length > 0;
			},
			highlighter: function (item) {
				var query = extractLast(this.query).replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, '\\$&');
				return item.replace(new RegExp('(' + query + ')', 'ig'), function ($1, match) {
					return '<strong>' + match + '</strong>';
				});
			},
			items: 10
		});

	});

	function split(val) {
		return val.split(/\s*[,;]\s*/);
	};

	function extractLast(term) {
		return split(term).pop();
	};

})();