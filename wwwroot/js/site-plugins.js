
// <![CDATA[
(function ($) {
    $.bootstrapModalConfirm = function (options) {
        var defaults = {
            caption: 'تائيد عمليات',
            body: 'آيا عمليات درخواستي اجرا شود؟',
            onConfirm: null,
            confirmText: 'تائيد',
            closeText: 'انصراف'
        };
        var options = $.extend(defaults, options);

        var confirmContainer = "#confirmContainer";
        var html = `
        <div class="modal fade" id="confirmContainer" tabindex="-1">
          <div class="modal-dialog">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">`+ options.caption + `</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
              </div>
              <div class="modal-body">
                <p>` + options.body + `</p>
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-primary" id="confirmBtn">` + options.confirmText + `</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">` + options.closeText + `</button>
              </div>
            </div>
          </div>
        </div>`;

        $(confirmContainer).remove();
        $(html).appendTo('body');
        $(confirmContainer).modal('show');

        $('#confirmBtn', confirmContainer).click(function () {
            if (options.onConfirm)
                options.onConfirm();
            $(confirmContainer).modal('hide');
        });
        $('#cancelBtn, #closeBtn', confirmContainer).click(function () {
            $(confirmContainer).modal('hide');
        });
    };
})(jQuery);
// ]]>

// <![CDATA[
(function ($) {
    $.bootstrapModalAjaxForm = function (options) {
        var defaults = {
            renderModalPartialViewUrl: null,
            renderModalPartialViewData: null,
            postUrl: '/',//for save data
            postData: null,
            loginUrl: '/login',
            dialogContainerId: 'dialogDiv',
            beforePostHandler: null,
            beforeSendGetPartialRequest: null,
            completeHandler: null,
            afterLoadModalHandler: null,
            errorHandler: null,
            postErrorHandler: null
        };

        var options = $.extend(defaults, options);
        var dialogContainerId = '#' + options.dialogContainerId;

        var validateForm = function (form) {
            //فعال سازي دستي اعتبار سنجي جي‌كوئري
            var val = form.validate();
            val.form();
            return val.valid();
        };

        var enableBootstrapStyleValidation = function () {
            $.validator.setDefaults({
                highlight: function (element, errorClass, validClass) {
                    if (element.type === 'radio') {
                        this.findByName(element.name).addClass(errorClass).removeClass(validClass);
                    } else {
                        $(element).addClass(errorClass).removeClass(validClass);
                        $(element).closest('.form-group').removeClass('has-success').addClass('has-error');
                    }
                    $(element).trigger('highlited');
                },
                unhighlight: function (element, errorClass, validClass) {
                    if (element.type === 'radio') {
                        this.findByName(element.name).removeClass(errorClass).addClass(validClass);
                    } else {
                        $(element).removeClass(errorClass).addClass(validClass);
                        $(element).closest('.form-group').removeClass('has-error').addClass('has-success');
                    }
                    $(element).trigger('unhighlited');
                }
            });
        }

        var enablePostbackValidation = function () {
            $('form').each(function () {
                $(this).find('div.form-group').each(function () {
                    if ($(this).find('span.field-validation-error').length > 0) {
                        $(this).addClass('has-error');
                    }
                });
            });
        }

        var processAjaxForm = function (dialog) {
            $('form', dialog).submit(function (e) {
                e.preventDefault();

                if (!validateForm($(this))) {
                    //اگر فرم اعتبار سنجي نشده، اطلاعات آن ارسال نشود
                    return false;
                }

                //در اينجا مي‌توان مثلا دكمه‌اي را غيرفعال كرد
                if (options.beforePostHandler)
                    options.beforePostHandler();

                //اطلاعات نبايد كش شوند
                $.ajaxSetup({ cache: false });

                let d = options.postData;
                if (!d) d = $(this).serialize();

                $.ajax({
                    url: options.postUrl,
                    type: "POST",
                    data: d,
                    success: function (result) {
                        if (result.success) {
                            $(dialogContainerId).modal('hide');
                            if (options.completeHandler)
                                options.completeHandler();
                        } else {
                            //در صورت خطا محتویات مدال جایگزین شود با پاسخ ارسالی
                            let res = $($.parseHTML(result)).children()[0].outerHTML;
                            $(dialogContainerId).html(res);

                            $.validator.unobtrusive.parse(dialogContainerId);
                            enablePostbackValidation();
                            processAjaxForm(dialogContainerId);

                            if (options.postErrorHandler)
                                options.postErrorHandler();
                        }
                    }
                });
                return false;
            });
        };

        //enableBootstrapStyleValidation(); //اعمال نكات خاص بوت استرپ جهت اعتبارسنجي يكپارچه با آن
        $.ajaxSetup({ cache: false });
        $.ajax({
            type: "GET",
            url: options.renderModalPartialViewUrl,
            data: options.renderModalPartialViewData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (jqXHR, settings) {
                if (options.beforeSendGetPartialRequest)
                    options.beforeSendGetPartialRequest()
            },
            complete: function (xhr, status) {
                var data = xhr.responseText;
                var data = xhr.responseText;
                if (xhr.status == 403 || xhr.status == 401) {
                    window.location = options.loginUrl + '?ReturnUrl=' + options.postUrl; //در حالت لاگين نبودن شخص اجرا مي‌شود
                }
                else if (status === 'error' || !data) {
                    if (options.errorHandler)
                        options.errorHandler();
                }
                else {
                    $(dialogContainerId).remove();
                    //var mainContainer = "<div id='" + options.dialogContainerId + "' class='modal fade'></div>";
                    $(data).appendTo('body');

                    //$(dialogContainerId).html(data); // دريافت پوياي اطلاعات مودال ديالوگ
                    //After page is loaded, the forms with jQuery validations enabled will be automatically parsed. If you are using dynamic forms, for example a FORM HTML returned from Ajax calls, you need to explicitly enable it by calling the following line as you add the dynamic HTML into HTML DOM:
                    $.validator.unobtrusive.parse(dialogContainerId); // فعال سازي اعتبارسنجي فرمي كه با ايجكس بارگذاري شده
                    enablePostbackValidation();
                    //$(dialogContainerId).modal('show');//bs v5
                    var myModal = new bootstrap.Modal(document.getElementById(options.dialogContainerId), {
                        backdrop: 'static',//با كليك كاربر روي صفحه، صفحه مودال بسته نمي‌شود
                        keyboard: true//Close by Esc
                    });
                    myModal.show();

                    if (options.afterLoadModalHandler)
                        options.afterLoadModalHandler();

                    // تحت نظر قرار دادن اين فرم اضافه شده
                    processAjaxForm(dialogContainerId);
                }
            }
        });
    };
})(jQuery);
// ]]>
/**
 * @summary     ConditionalPaging
 * @description Hide paging controls when the amount of pages is <= 1
 * @version     1.0.0
 * @file        dataTables.conditionalPaging.js
 * @author      Matthew Hasbach (https://github.com/mjhasbach)
 * @contact     hasbach.git@gmail.com
 * @copyright   Copyright 2015 Matthew Hasbach
 *
 * License      MIT - http://datatables.net/license/mit
 *
 * This feature plugin for DataTables hides paging controls when the amount
 * of pages is <= 1. The controls can either appear / disappear or fade in / out
 *
 * @example
 *    $('#myTable').DataTable({
 *        conditionalPaging: true
 *    });
 *
 * @example
 *    $('#myTable').DataTable({
 *        conditionalPaging: {
 *            style: 'fade',
 *            speed: 500 // optional
 *        }
 *    });
 */

(function (window, document, $) {
    $(document).on('init.dt', function (e, dtSettings) {
        if (e.namespace !== 'dt') {
            return;
        }

        var options = dtSettings.oInit.conditionalPaging || $.fn.dataTable.defaults.conditionalPaging;

        if ($.isPlainObject(options) || options === true) {
            var config = $.isPlainObject(options) ? options : {},
                api = new $.fn.dataTable.Api(dtSettings),
                speed = 'slow',
                conditionalPaging = function (e) {
                    var $paging = $(api.table().container()).find('div.dataTables_paginate'),
                        pages = api.page.info().pages;
                    var $info = $(api.table().container()).find('div.dataTables_info');//sirous

                    if (e instanceof $.Event) {
                        if (pages <= 1) {
                            if (config.style === 'fade') {
                                $paging.stop().fadeTo(speed, 0);
                                $info.stop().fadeTo(speed, 0);
                            }
                            else {
                                $paging.css('visibility', 'hidden');
                                $info.css('visibility', 'hidden');
                            }
                        }
                        else {
                            if (config.style === 'fade') {
                                $paging.stop().fadeTo(speed, 1);
                                $info.stop().fadeTo(speed, 1);
                            }
                            else {
                                $paging.css('visibility', '');
                                $info.css('visibility', '');
                            }
                        }
                    }
                    else if (pages <= 1) {
                        if (config.style === 'fade') {
                            $paging.css('opacity', 0);
                            $info.css('opacity', 0);
                        }
                        else {
                            $paging.css('visibility', 'hidden');
                            $info.css('visibility', 'hidden');
                        }
                    }
                };

            if (config.speed !== undefined) {
                speed = config.speed;
            }

            conditionalPaging();

            api.on('draw.dt', conditionalPaging);
        }
    });
})(window, document, jQuery);