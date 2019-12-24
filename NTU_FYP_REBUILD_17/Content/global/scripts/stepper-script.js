/*jslint browser: true*/
/*global $, jQuery, alert*/
includeJs("~/Content/global/scripts/inputChecks.js");


(function($) {
    'use strict';

    $(function() {

        $(document).ready(function() {
            function triggerClick(elem) {
                $(elem).click();
            }
            var $progressWizard = $('.stepper'),
                $tab_active,
                $tab_prev,
                $tab_next,
                $btn_prev = $progressWizard.find('.prev-step'),
                $btn_next = $progressWizard.find('.next-step'),
                $tab_toggle = $progressWizard.find('[data-toggle="tab"]'),
                $tooltips = $progressWizard.find('[data-toggle="tab"][title]');

            // To do:
            // Disable User select drop-down after first step.
            // Add support for payment type switching.

            //Initialize tooltips
            $tooltips.tooltip();

            //Wizard
            $tab_toggle.on('show.bs.tab', function(e) {
                var $target = $(e.target);

                if (!$target.parent().hasClass('active, disabled')) {
                    $target.parent().prev().addClass('completed');
                }
                if ($target.parent().hasClass('disabled')) {
                    return false;
                }
            });

             //$tab_toggle.on('click', function(event) {
             //    event.preventDefault();
             //    event.stopPropagation();
             //    return false;
             //});

            $btn_next.on('click', function () {
                var curStep = $(this).closest(".tab-pane");

                $tab_active = $progressWizard.find('.active');

                $tab_active.next().removeClass('disabled');

                $tab_next = $tab_active.next().find('a[data-toggle="tab"]');

                var curInputs = curStep.find("input[type='text'],input[type='url']");
                var isValid = true;


                $(".form-group").removeClass("has-error");

                for (var i = 0; i < curInputs.length; i++) {
                    if ($(curInputs[i]).attr("id") == "NRIC") {
                        var nric = $(curInputs[i]).val();
                        var type = $(curInputs[i]).attr("xtype");

                        if (!nric.trim() == "") {

                            if (!nricValidation(nric, type)) {
                                isValid = false;
                                $(curInputs[i]).closest(".form-group").addClass("has-error");
                            }
                        }
                        
                    }

                    if ($(curInputs[i]).attr("dateJoin") == "dateJoin") {

                        if (!dateCheckaddPatient()) {
                            isValid = false;
                            $(curInputs[i]).closest(".form-group").addClass("has-error");
                        }
                    }

                    if (!curInputs[i].validity.valid) {     
                        isValid = false;
                        $(curInputs[i]).closest(".form-group").addClass("has-error");
                    }

                   }

                
             

                if (isValid) {
                triggerClick($tab_next);
                }

            });
            $btn_prev.click(function() {
                $tab_active = $progressWizard.find('.active');
                $tab_prev = $tab_active.prev().find('a[data-toggle="tab"]');
                triggerClick($tab_prev);
            });
        });
    });

}(jQuery, this));


function includeJs(jsFilePath) {
    var js = document.createElement("script");

    js.type = "text/javascript";
    js.src = jsFilePath;

    document.body.appendChild(js);
}
