if ($('body[data-active-page="Viewer"]')) {

	(function () {

		$(function () {

			$("#image").ready(updateZoomIndicator);
			$(window).resize(updateZoomIndicator);
			//	.keyup(function (e) {
			//		if ((e.ctrlKey || e.metaKey) && e.keyCode == 37) {
			//			var prewUrl = $("#prev-button").attr("href");
			//			if (prewUrl != undefined)
			//				location.href = prewUrl;
			//		}
			//		if ((e.ctrlKey || e.metaKey) && e.keyCode == 39) {
			//			var nextUrl = $("#next-button").attr("href");
			//			if (nextUrl != undefined)
			//				location.href = nextUrl;
			//		}
			//	});

			$("input[type='radio'][name='userScore']").change(function () {
				var $this = $(this);
				var score = $this.prop("value");
				$.post("/Pictures/Rate", { pictureID: $this.data("pictureId"), userScore: score }, function (data) {
					$("#score-value").html(data);
				});
			});
			
			$("#favorite-checkbox").change(function () {
				var $this = $(this);
				$.post("/Pictures/SetFavorited", { pictureID: $this.data("pictureId"), favorited: $this.prop("checked") } );
			});

			$("#viewer-link-box, #direct-link-box").click(function () {
				this.select();
			});

			$("#comment-textarea")
				.keyup(function (e) {
					if ((e.ctrlKey || e.metaKey) && (e.keyCode == 10 || e.keyCode == 13)) {
						$("#comment-form").submit();
					}
				});
		});

	})();

	function updateZoomIndicator() {
		var image = $("#image");
		var zoomRatio = Math.round(image.width() / image.data("originalWidth") * 100);
		if (zoomRatio > 0 && zoomRatio < 100) {
			$("#zoom-indicator-value").text(zoomRatio);
			$("#zoom-indicator").show();
		} else {
			$("#zoom-indicator").hide();
		}
	}

}