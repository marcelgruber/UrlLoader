// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(function() {

    // TODO: convert to module and bundle
    // TODO: store all selectors / elements in single location
    // TODO: add loading message / UI cue
    // TODO: prevent user from calling loadUrl too often
    // TODO: finish full conversion to Vue
    // TODO: handle multiple button clicks in succession
    // TODO: break this wall of code into smaller functions
    // TODO: more validation of URL
    // TODO: show validation message 
    // TODO: better handling of case where there is only 1 image
    // TODO: more error checking 
    // TODO: handle case where gallery view does not display images until clicking next or previous
    // TODO: better handling of edge cases
    // TODO: better UI cues for edge cases
    // TODO: allow submit via enter key

    function toggleSubmit () {
        let $button = $('#load-url-form button');

        if ($button.prop('disabled') === true) {
            $button.prop('disabled', false);
        } else {
            $button.prop('disabled', true);
        }
    }

    function clearView () {
        let $imageGallery = $('.load-url-result .image-gallery');
        if ($imageGallery.hasClass('slick-initialized')) {
            $imageGallery.slick('unslick');
        }
                
        $imageGallery.empty();
        $('#load-url-form .status').html('');

        $("#word-graph-chart").remove();
        $(".word-graph").append('<canvas id="word-graph-chart" width="400" height="400"></canvas>');
    }

    function updateStatusMessage(message) {
        $('#load-url-form .status').html(message);
    }

    function renderChart(topTenWords, topTenWordCounts) {
        var ctx = document.getElementById('word-graph-chart').getContext('2d');
        var wordCountChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: topTenWords,
                datasets: [{
                    label: '# of Instances',
                    data: topTenWordCounts,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(75, 192, 192, 0.2)',
                        'rgba(153, 102, 255, 0.2)',
                        'rgba(255, 159, 64, 0.2)'
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(153, 102, 255, 1)',
                        'rgba(255, 159, 64, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    var app = new Vue({
        el: '#vue-url-loader-app',
        data: {
            wordCountTitle: 'Words'
        },
        methods: {
            loadUrl (e) {
                var app = this;

                const $inputEl = $('.js-url-input');

                console.log($inputEl.val());
                const shouldLoadUrl = $inputEl.val() && !$inputEl.val() == '';
                if (!shouldLoadUrl) {
                    return;
                }

                toggleSubmit();
                updateStatusMessage('Loading...');

                $('.load-url-result').fadeOut(0); // fade out immediately for a cleaner refresh

                clearView();

                const loadUrlRoute = "/Home/LoadUrl";
                const apiUrl = location.protocol + '//' + location.host + loadUrlRoute + "?url=";
                const inputUrl = $inputEl.val();
                let $imageGallery = $('.load-url-result .image-gallery');

                $.ajax({
                    type: "POST",
                    url: apiUrl + inputUrl,
                    success: function(result) {

                        if (!result || result.success === false) {
                            updateStatusMessage(result.message);
                            toggleSubmit();
                            
                            return;
                        }

                        if (result.images) {

                            if (result.images.length > 0) {
                                $.each(result.images,
                                    function(index, value) {
                                        $imageGallery.append('<img src="' + value + '">');
                                    });
                            } else {
                                $imageGallery.append('<p>The URL contains no images.</p>');
                            }

                            $imageGallery.slick({
                                infinite: true,
                                slidesToShow: 3,
                                slidesToScroll: 3
                            });
                        }

                        if (result.words) {
                            app.wordCountTitle = result.words.length + " Words";

                            var topTenWords = [];
                            var topTenWordCounts = [];
                            const maxChartableWords = 10;

                            $.each(result.words, function(index, val) {
                                if (index > maxChartableWords - 1) return;

                                topTenWords.push(val.word);
                                topTenWordCounts.push(val.count);
                            });

                            renderChart(topTenWords, topTenWordCounts);
                        }

                        setTimeout(function() {
                            $('.load-url-result').fadeIn();
                        }, 1000);
                        
                        toggleSubmit();
                    }
                });
            }
        }
    });
});
