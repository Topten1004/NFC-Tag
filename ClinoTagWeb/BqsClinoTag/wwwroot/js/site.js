// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $("input").attr("autocomplete", "off");
    $("select").attr("autocomplete", "off");
    $('[data-toggle="tooltip"]').tooltip({
        html: true,
        template: '<div class="tooltip" role="tooltip"><div class="tooltip-inner" style="background-color: white;"></div></div>'
    });

    $('[data-toggle="copytooltip"]').click(function () {
        var tooltip = $(this);
        // Create tooltip
        tooltip.tooltip({
            html: true,
            template: '<div class="tooltip" role="tooltip"><div class="tooltip-inner" style="background-color: white;"></div></div>',
            trigger: 'manual'
        });
        // Toggle tooltip
        tooltip.tooltip('toggle');
        // Remove tooltip after 2 seconds
        setTimeout(function () {
            tooltip.tooltip('hide');
        }, 1600);
    });

    $('.copy').on('click', function () {
        var value = $(this).attr('value');

        var content = $('#' + value).text().trim();

        navigator.clipboard.writeText(content)
            .then(function () {
                console.log('Text copied to clipboard successfully.');
            })
            .catch(function (error) {
                console.error('Unable to copy text to clipboard:', error);
            });
    });

});
