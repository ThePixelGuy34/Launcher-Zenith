// Copyright 1998-2017 Epic Games, Inc. All Rights Reserved.

var modalInfo = {
	title: "Test Title",
	messages: [
		"This is a test message for testing the dialog via a standard browser.",
		"Another text message for testing the dialog via a standard web browser.  This one is a bit longer so we can see how things wrap.",
		"Third message for testing dialog.  Also a multiline comment to test the wrap of the text.",
		],
	imageurl: "",
	commands: [{id: "confirmId", displayname: "confirm"}, {id: "cancelId", displayname: "cancel"}]	
};

var config = {
    redeemFadeInDelay: 50,
	redeemFadeOutDelay: 300
}

function closeModal() {
    console.log("closeModal()");
        
    $('#web-modal').fadeOut(config.redeemFadeOutDelay, function() {    
    	if (window.ue && window.ue.realidverification) {
			window.ue.realidverification.executecommand("CancelCommand");
        }
        else {
        	window.close();
        }
    });
}

function refreshModal() {
	console.log("refreshModal()");
	if(modalInfo)
	{
		var $sidebarImgDefault = $('.sidebar-image-default');
		var $sidebarImgOverride = $('.sidebar-image-override');

		// Show the default image or the override image passed in
		if (!modalInfo.imageurl || modalInfo.imageurl === "") {
		    $sidebarImgOverride.hide();        
		    $sidebarImgDefault.show();
		} else {
		    $sidebarImgDefault.hide();
		    $sidebarImgOverride.attr('src', "");
		    $sidebarImgOverride.attr('src', modalInfo.imageurl)
		    $sidebarImgOverride.show();
		}

		// Set the modal text fields
		var $modalTitle = $('#modalTitle');
		var $modalMessages = $('.modal-panel');

		$modalTitle.html(modalInfo.title || "");

		for(let message of modalInfo.messages)
		{
			var $newMessage = $('<p>' + message + '</p>');
			$newMessage.appendTo($modalMessages);
		}

		// Generate modal buttons
		var $modalAction = $('.modal-action');

		for(let command of modalInfo.commands)
		{
			var $newButton = $('<button id="' + command.id + '" class="btn">'+ command.displayname +'</button>');
			$newButton.click(function () {
				$('#web-modal').fadeOut(config.redeemFadeOutDelay, function(localButtonId) {    
					if (window.ue && window.ue.realidverification) {
						window.ue.realidverification.executecommand(this.id);
					}
					else {
						window.close();
					}
				}.bind(this));
			});
			$newButton.appendTo($modalAction);
		}
	}
}

$(document).ready(function () {

	console.log("DocumentReady()");

    // Let's hide everything until we've completed loading and it's ready    
	var $overlay = $('#web-modal');
    $overlay.hide();
    
	var $sidebarImgOverride = $('.sidebar-image-override');
	var $sidebarImgDefault = $('.sidebar-image-default');
	var $closeBtn = $('.close-btn');


	// Close x button
	$closeBtn.click(function(){
		closeModal();
	});

	$(document).keyup(function (e) {
		// If user presses ESC key, we close the modal
		if (e.keyCode == 27) { // escape key maps to keycode `27`
			closeModal();
		}
	});

	var bootstrap = function () {
		console.log("bootstrap()");
		// We're ready to go
		$overlay.fadeIn(config.redeemFadeInDelay);
	}

	if (!window.ue) {
		document.body.style.backgroundColor = "black";
		refreshModal();
		bootstrap();
	}
	else {
		// Disable right click context menu
		window.addEventListener("contextmenu",
			function (e) {
				e.preventDefault()
				return false;
			});

		var postJsBridgeInit = function () {
			console.log("postJsBridgeInit()");
			if (window.ue.realidverification) {
				window.ue.realidverification.getmodalinfo().then(newModalInfo => {
					modalInfo = newModalInfo;
					refreshModal();
					bootstrap();
				});
			}
			else {
				console.log("Could not locate window.ue.realidverification object, trying again in 50ms.");
				setTimeout(postJsBridgeInit, 50);
			}
		};
		postJsBridgeInit();
	}
});
