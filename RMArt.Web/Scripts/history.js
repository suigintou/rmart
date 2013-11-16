if ($('body[data-active-page="History"]')) {

	(function () {

		$(function () {

			$(".history-event")
				.mouseenter(function () {
					$('.history-event[data-id="' + $(this).data('overridenby') + '"]').addClass("next");
					$('.history-event[data-overridenby="' + $(this).data('id') + '"]').addClass("prev");
				})
				.mouseleave(function () {
					$(".history-event").removeClass("next").removeClass("prev");
				});

		});

	})();

}