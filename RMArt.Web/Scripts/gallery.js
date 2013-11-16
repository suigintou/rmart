if ($('body[data-active-page="Gallery"]')) {

	(function () {

		$(function () {

			$('.thumbnail')
				.mouseenter(function () {
					var ids = $(this).data('tagIds').toString().split(' ');
					for (var i in ids)
						$('.tag[data-id="' + ids[i] + '"]').addClass("highlight");
				})
				.mouseleave(function () { $(".tag").removeClass("highlight"); });

			$(".tag")
				.mouseenter(function () {
					$('.thumbnail[data-tag-ids~="' + $(this).data("id") + '"]').addClass("highlight");
				})
				.mouseleave(function () { $(".thumbnail").removeClass("highlight"); });

			$("#select-mode-button")
				.click(function () {
					setSelectMode(!$(this).hasClass("active"));
				});
			$("#select-all-button")
				.click(function () {
					ensureSelectMode();
					setAllThumbsSelected(true);
				});
			$("#deselect-all-button")
				.click(function () {
					ensureSelectMode();
					setAllThumbsSelected(false);
				});

		});

		function setThumbSelected(thumb, isSelected) {
			if (isSelected) {
				$(thumb).addClass("selected").find(".thumb-checkbox").attr("checked", "checked");
				$("#edit-button").removeAttr("disabled");
			} else {
				$(thumb).removeClass("selected").find(".thumb-checkbox").removeAttr("checked");
				if (!$("#thumbs li").hasClass("selected"))
					$("#edit-button").attr("disabled", "disabled");
			}
			return false;
		}

		function toggleThumbSelected(e) {
			if (e.which != 1 || e.ctrlKey)
				return true;
			if ($(this).hasClass("selected")) {
				setThumbSelected(this, false);
			} else {
				setThumbSelected(this, true);
			}
			return false;
		}

		function setAllThumbsSelected(isSelected) {
			$("#thumbs li").each(function (i, e) { setThumbSelected(e, isSelected); });
		}

		function setSelectMode(isEnabled) {
			if (isEnabled) {
				$(".thumb-checkbox:checked").parents("li").addClass("selected");
				$("#thumbs li").click(toggleThumbSelected);
				if ($("#thumbs li").hasClass("selected"))
					$("#edit-button").removeAttr("disabled");
			} else {
				$("#thumbs li.selected").removeClass("selected");
				$("#thumbs li").unbind("click", toggleThumbSelected);
				$("#edit-button").attr("disabled", "disabled");
			}
		}

		function ensureSelectMode() {
			$("#select-mode-button:not(.active)").click();
		}

	})();

}