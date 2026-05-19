function initDateTimePicker() {
    $('.date').dtDateTime({
        format: 'YYYY/MM/D', //need moment.js
        buttons: {
            today: true,
            clear: true
        }
    });

    $('.datetime').dtDateTime({
        format: 'YYYY/MM/D HH:mm',
        buttons: {
            today: true,
            clear: true
        }
    });
}

//انتخاب فرد مورد نظر در کومبو
function initDropDown() {
    return;
    $('.select-onload').each(function () {
        var s = $(this).attr('value');
        $(this).val(s);
    });
}

function addSelectToSearch(selector, paramName = selector) {
    //نکته استفاده: حتما همه آیتم‌ها مقدار داشته باشند ولو خالی
    var sel = $("select[name='" + selector + "']");
    if (sel.length == 0) sel = $("select[id='" + selector + "']");
    val = sel.val();
    if (val)
        return paramName + '=' + val + ',';
    else
        return '';
}

function formatUSD(value) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }).format(value);
}

//datatable
// تنظیمات دیگر عمومی که می‌خوای برای همه‌ی جداول باشه مثلاً:
$.extend(true, $.fn.dataTable.defaults, {
    language: { url: '/lib/dataTables-1.12.1/fa.json?v1' },
    //dom: "rtip",//https://datatables.net/reference/option/dom
    dom: "rt<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
    serverSide: true,
    processing: true,
    order: [[0, 'desc']],
    responsive: true,
    pageLength: 20,
    conditionalPaging: true
});

function renderActionButton(keyName, buttonConfigs = []) {
    return {
        data: null,
        orderable: false,
        searchable: false,
        responsivePriority: -1,
        render: function (data, type, row) {
            var id = row[keyName];
            //اگر شناسه رشته است آنرا در کوتیشن قرار میدهد
            var formattedId = typeof id === 'number' ? id : '"' + id.replace(/"/g, '\\"') + '"';

            let buttonsHtml = '';
            for (const config of buttonConfigs) {
                // فراخوانی تابع سازنده دکمه از طریق پیکربندی
                const renderer = config.renderer || generateButtonHtml;
                buttonsHtml += renderer(formattedId, config); // تابع رندرر و خود کانفیگ را پاس می‌دهیم
            }
            return buttonsHtml;
        }
    }
}
// تابع سازنده دکمه که پیکربندی را هم دریافت می‌کنند
function generateButtonHtml(id, config) {
    // استفاده از کلاس‌های CSS و آیکون از پیکربندی
    let classes = `px-2 ${config.extraClass || ''}`;
    let iconHtml = `<i class='${config.iconClass}'></i>`;
    let titleAttribute = config.title ? `title='${config.title}'` : '';

    return `<a href='javascript:${config.actionName}(${id})' class='${classes}' ${titleAttribute}>${iconHtml}</a>`;
}

function renderCheckBox(value) {
    return value ? '<i class="far fa-check-square"></i>' : '<i class="far fa-square"></i>';
}
//هنگام نمایش مجموع لازم است به اندازه تعداد ستونها در فوتر خانه ایجاد کنیم بخاطر اینکه خودکار اینکار انجام شود اینجا نوشتیم
//لازم است فوتر قبل ایجاد جدول اضافه شود تا بشناسد
function addTableFooter(columnCount, tableId = 'myTable') {
    var tfoot = $('<tfoot class="bg-warning"><tr></tr></tfoot>');
    for (var i = 0; i < columnCount; i++) {
        tfoot.find('tr').append('<td></td>');
    }
    $('#' + tableId).append(tfoot);
}

function renderFooterHtml(json, {
    totalField = 'total',
    pageTotalField = 'pageTotal',
    totalDinarField = 'totalDinar',
    pageTotalDinarField = 'pageTotalDinar' }) {

    let html = '<div dir="ltr">';
    let hasDinar = pageTotalDinarField && totalDinarField && json[pageTotalDinarField] !== undefined;
    let hasDollar = pageTotalField && totalField && json[pageTotalField] !== undefined;
    let twoLine = hasDinar && hasDollar ? '' : '<br/>';

    if (hasDinar) {
        html += (json[pageTotalDinarField] ?? 0).toLocaleString() + 'D' + twoLine + ' (' + (json[totalDinarField] ?? 0).toLocaleString() + ' total)<br/>';
    }
    if (hasDollar) {
        html += (json[pageTotalField] ?? 0).toLocaleString() + '$' + twoLine + ' (' + (json[totalField] ?? 0).toLocaleString() + ' total)';
    }
    html += '</div>';
    return html;
}
/**
 * Creates a drawCallback for one column's sum footer.
 * 
 * @param {Object} config - Configuration.
 * @param {number} config.col - Target column index.
 * @param {string} [config.pageTotalField='pageTotal'] - Field for current page (USD).
 * @param {string} [config.totalField='total'] - Field for total (USD).
 * @param {string} [config.pageTotalDinarField='pageTotalDinar'] - Field for current page (Dinar).
 * @param {string} [config.totalDinarField='totalDinar'] - Field for total (Dinar).
 * @returns {function} DataTable drawCallback
 */
function setupFooterTotal(config) {
    return function () {
        const api = this.api();
        const json = api.ajax.json();
        if (!json) return;

        const html = renderFooterHtml(json, config);
        $(api.column(config.col).footer()).html(html);
    };
}
/**
 * Creates a drawCallback for multiple columns' sum footers.
 * 
 * @param {Array<Object>} columns - Array of config objects.
 * @param {number} columns[].col - Target column index.
 * @param {string} [columns[].pageTotalField='pageTotal']
 * @param {string} [columns[].totalField='total']
 * @param {string} [columns[].pageTotalDinarField='pageTotalDinar']
 * @param {string} [columns[].totalDinarField='totalDinar']
 * @returns {function} DataTable drawCallback
 */
function setupMultipleFooterTotal(columns) {
    return function () {
        const api = this.api();
        const json = api.ajax.json();
        if (!json) return;

        columns.forEach(col => {
            const html = renderFooterHtml(json, col);
            $(api.column(col.col).footer()).html(html);
        });
    };
}
