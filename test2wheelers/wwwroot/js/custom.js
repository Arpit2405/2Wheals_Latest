jQuery(function($) {
  "use strict";


	/* ----------------------------------------------------------- */
	/*  Fixed header
	/* ----------------------------------------------------------- 


	/* ----------------------------------------------------------- */
	/*  Mobile Menu
	/* ----------------------------------------------------------- */

	jQuery(".nav.navbar-nav li a").on("click", function() { 
		jQuery(this).parent("li").find(".dropdown-menu").slideToggle();
		jQuery(this).find("li i").toggleClass("fa-angle-down fa-angle-up");
	});


	$('.nav-tabs[data-toggle="tab-hover"] > li > a').hover( function(){
    	$(this).tab('show');
	});



	window.addEventListener("scroll", function () {
		const nav = document.querySelector(".main-nav");
		if (window.scrollY > 130) {
			nav.classList.add("sticky-top");
		} else {
			nav.classList.remove("sticky-top");
		}
	});

	/* ----------------------------------------------------------- */
  	/*  Site search
  	/* ----------------------------------------------------------- */



	 $('.nav-search').on('click', function () {
		 $('.search-block').fadeIn(350);
	});

	 $('.search-close').on('click', function(){
			  $('.search-block').fadeOut(350);
	 });



  	/* ----------------------------------------------------------- */
  	/*  Owl Carousel
  	/* ----------------------------------------------------------- */

  	//Trending slide

  	$(".trending-slide").owlCarousel({

			loop:true,
			animateIn: 'fadeIn',
			autoplay:true,
			autoplayTimeout:3000,
			autoplayHoverPause:true,
			nav:true,
			margin:30,
			dots:false,
			mouseDrag:false,
			slideSpeed:500,
			navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
			items : 1,
			responsive:{
			  0:{
					items:1
			  },
			  600:{
					items:1
			  }
			}

		});


  	//Featured slide

		$(".featured-slider").owlCarousel({

			loop:true,
			animateOut: 'fadeOut',
			autoplay:true,
			autoplayTimeout:7000,
			autoplayHoverPause:true,
			nav:true,
			margin:0,
			dots:false,
			mouseDrag:true,
			touchDrag:true,
			slideSpeed:500,
			navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
			items : 1,
			responsive:{
			  0:{
					items:1
			  },
			  600:{
					items:1
			  }
			}

		});

		//Latest news slide

		$(".latest-news-slide").owlCarousel({

			loop:false,
			animateIn: 'fadeInLeft',
			autoplay:false,
			autoplayHoverPause:true,
			nav:true,
			margin:10,
			dots:false,
			mouseDrag:true,
			slideSpeed:500,
			navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
			items : 3,
			responsive:{
			  0:{
					items:1
			  },
			  600:{
					items:5
			  }
			}

		});

		//Latest news slide


		//Latest news slide

		$(".more-news-slide").owlCarousel({

			loop:false,
			autoplay:false,
			autoplayHoverPause:true,
			nav:false,
			margin:30,
			dots:true,
			mouseDrag:false,
			slideSpeed:500,
			navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
			items : 1,
			responsive:{
			  0:{
					items:1
			  },
			  600:{
					items:1
			  }
			}

		});

		
  const swiper = new Swiper(".myVerticalSlider", {
    direction: "vertical",
    slidesPerView: 4,
    spaceBetween: 5,
    mousewheel: true,
    navigation: {
      nextEl: ".swiper-button-next",
      prevEl: ".swiper-button-prev",
    },
  });
 var owl = $(".custom-owl-slider");
  owl.owlCarousel({
    loop:true,
    margin:20,
    items:4,
	dots:false,
    responsive:{
      0:{ items:1 },
      576:{ items:2 },
      768:{ items:3 },
      992:{ items:4 },
      1200:{ items:8 }
    }
  });

  // Custom arrows functionality
  $(".custom-nav.next").click(function(){
    owl.trigger('next.owl.carousel');
  });
  $(".custom-nav.prev").click(function(){
    owl.trigger('prev.owl.carousel');
  });
  
  document.querySelectorAll('.myVerticalSlider .nav-link').forEach(link => {
    link.addEventListener('click', function(e){
      e.preventDefault();
      const target = this.getAttribute('href');

      // Activate tab content
      document.querySelectorAll('.tab-pane').forEach(p => p.classList.remove('active','show'));
      document.querySelector(target).classList.add('active','show');

      // Activate nav link
      document.querySelectorAll('.myVerticalSlider .nav-link').forEach(l => l.classList.remove('active'));
      this.classList.add('active');
    });
  });

		$(".post-slide").owlCarousel({

			loop:true,
			animateOut: 'fadeOut',
			autoplay:false,
			autoplayHoverPause:true,
			nav:true,
			margin:30,
			dots:false,
			mouseDrag:false,
			slideSpeed:500,
			navText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
			items : 1,
			responsive:{
			  0:{
					items:1
			  },
			  600:{
					items:1
			  }
			}

		});

		

	/* ----------------------------------------------------------- */
	/*  Popup
	/* ----------------------------------------------------------- */
	  $(document).ready(function(){

			$(".gallery-popup").colorbox({rel:'gallery-popup', transition:"fade", innerHeight:"500"});

			$(".popup").colorbox({iframe:true, innerWidth:600, innerHeight:400});

	  });

	
	/* ----------------------------------------------------------- */
	/*  Contact form
	/* ----------------------------------------------------------- */

	$('#contact-form').submit(function(){

		var $form = $(this),
			$error = $form.find('.error-container'),
			action  = $form.attr('action');

		$error.slideUp(750, function() {
			$error.hide();

			var $name = $form.find('.form-control-name'),
				$email = $form.find('.form-control-email'),
				$subject = $form.find('.form-control-subject'),
				$message = $form.find('.form-control-message');

			$.post(action, {
					name: $name.val(),
					email: $email.val(),
					subject: $subject.val(),
					message: $message.val()
				},
				function(data){
					$error.html(data);
					$error.slideDown('slow');

					if (data.match('success') != null) {
						$name.val('');
						$email.val('');
						$subject.val('');
						$message.val('');
					}
				}
			);

		});

		return false;

	});


	/* ----------------------------------------------------------- */
	/*  Back to top
	/* ----------------------------------------------------------- */

		$(window).scroll(function () {
			if ($(this).scrollTop() > 50) {
				 $('#back-to-top').fadeIn();
			} else {
				 $('#back-to-top').fadeOut();
			}
		});

		// scroll body to 0px on click
		$('#back-to-top').on('click', function () {
			 $('#back-to-top').tooltip('hide');
			 $('body,html').animate({
				  scrollTop: 0
			 }, 800);
			 return false;
		});
		
		$('#back-to-top').tooltip('hide');


  var txt = "Search for Latest bikes..";
    var txtLen = txt.length;
    var char = 0;
    var timeOut;

    function typeIt() {
      var humanize = Math.round(Math.random() * (200 - 30)) + 30;
      timeOut = setTimeout(function () {
        char++;
        var type = txt.substring(0, char);
        $('.sf-searchbox').attr('placeholder', type + '|');

        if (char < txtLen) {
          typeIt();
        } else {
          // Remove the blinking | and restart after delay
          $('.sf-searchbox').attr('placeholder', txt);
          setTimeout(() => {
            char = 0;
            $('.sf-searchbox').attr('placeholder', '|');
            typeIt();
          }, 2000); // Wait 2 seconds before restarting
        }
      }, humanize);
    }

    // Start typing effect
    $('.sf-searchbox').attr('placeholder', '|');
    typeIt();

});
$(".mega-dropdown").hover(
  function() { // mouse enter
    $(".bgblk").addClass("active");
  },
  function() { // mouse leave
    $(".bgblk").removeClass("active");
  }
);

// stories   

function generateAMPWebStory() {
  const items = document.querySelectorAll('#latest-news-slide .item');
  let ampStory = `
  <!DOCTYPE html>
  <html ⚡>
  <head>
    <meta charset="utf-8">
    <title>2 wheal Web Stories</title>
    <link rel="canonical" href="self.html" />
    <meta name="viewport" content="width=device-width,minimum-scale=1,initial-scale=1">
	<link rel="icon" href="../images/favicon.ico" type="image/x-icon">
    <script async src="https://cdn.ampproject.org/v0.js"></script>
    <script async src="https://cdn.ampproject.org/v0/amp-story-1.0.js"></script>
    <script async custom-element="amp-video" src="https://cdn.ampproject.org/v0/amp-video-0.1.js"></script>
    <style amp-boilerplate>body{visibility:hidden}</style>
    <noscript><style amp-boilerplate>body{visibility:visible}</style></noscript>
   <style amp-custom>
body {
  background: linear-gradient(135deg, #ff416c, #ff4b2b);
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  margin: 0;
  min-height: 100vh;
  text-align: center;
  padding: 40px 20px 100px; /* bottom padding to avoid overlap with button */
}

h1 {
  font-size: 3rem;
  color: #fff;
  margin-bottom: 15px;
  text-shadow: 3px 3px 10px rgba(0,0,0,0.4);
}

p {
  font-size: 1.2rem;
  color: #f8f8f8;
  margin-bottom: 50px;
  line-height: 1.6;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

.bottom-link {
  position: fixed; 
  bottom: 20%;
  left: 50%;
  transform: translateX(-50%);
  background: linear-gradient(135deg, #ff6a00, #e00);
  padding: 14px 32px;
  border-radius: 35px;
  color: #fff;
  text-decoration: none;
  font-weight: bold;
  font-size: 17px;
  width: max-content;
  font-family: 'Cursive', sans-serif;
  box-shadow: 0 8px 15px rgba(0,0,0,0.3);
  transition: all 0.4s ease, box-shadow 0.4s ease;
  overflow: hidden;
  z-index: 999; /* stay above everything */
}

.bottom-link::before {
  content: "";
  position: absolute;
  top: 0;
  left: -75%;
  width: 50%;
  height: 100%;
  background: rgba(255,255,255,0.3);
  transform: skewX(-25deg);
  transition: all 0.5s ease;
}

.bottom-link:hover::before {
  left: 125%;
}

.bottom-link:hover {
  transform: translateX(-50%) translateY(-5px);
  box-shadow: 0 15px 25px rgba(0,0,0,0.5);
  background: linear-gradient(135deg, #ff4b2b, #ff6a00);
}
</style>

  </head>
  <body>
    <amp-story standalone
      title="Latest Bike Stories"
      publisher="Your Site"
      publisher-logo-src="images/logo.png"
      poster-portrait-src="${items[0].dataset.img || items[0].dataset.video}">
  `;

  items.forEach((item, index) => {
    const title = item.dataset.title;
    const desc = item.dataset.desc;
    const img = item.dataset.img;
    const video = item.dataset.video;
    const link = item.dataset.link;

    ampStory += `<amp-story-page id="page${index+1}">`;
    ampStory += `<amp-story-grid-layer template="fill">`;

    if(video){
      // Video slide
      ampStory += `<amp-video src="${video}" width="720" height="1280" layout="responsive" autoplay loop muted></amp-video>`;
    } else if(img){
      // Image slide
      ampStory += `<amp-img src="${img}" width="720" height="1280" layout="responsive"></amp-img>`;
    }

    ampStory += `</amp-story-grid-layer>`; // close fill
    ampStory += `<amp-story-grid-layer template="vertical">`;
    ampStory += `<h1>${title}</h1><p>${desc}</p>`;
    if(link){
      ampStory += `<a class="bottom-link" href="${link}" target="_blank">Swipe Up</a>`;
    }
    ampStory += `</amp-story-grid-layer></amp-story-page>`;
  });

  ampStory += `</amp-story></body></html>`;

  const newWindow = window.open();
  newWindow.document.write(ampStory);
  newWindow.document.close();
}

document.querySelectorAll('#latest-news-slide .item').forEach(item => {
  item.addEventListener('click', generateAMPWebStory);
});
