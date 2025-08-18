var selectedIDs = '';
var gridKey = '';
var appSubDir = '';
var timer;
var groupedColumns = '';
var showPopup = true;
var iframe;
var mainPopupControl = new Object(); //Dictionary

function OnBeginCallback(s, e) {
    e.customArgs["selectedIDs"] = selectedIDs;
}

function SelectionChanged(s, e) {
    selectedIDs = s.GetRowKey(e.visibleIndex);
    //s.GetSelectedFieldValues("gridKey", GetSelectedFieldValuesCallback);    
}

function GetSelectedFieldValuesCallback(values) {
    try {
        selectedIDs = "";
        for (var i = 0; i < values.length; i++) {
            var strValue = values[i] + "";
            selectedIDs += strValue.replace(/,/g, '|');
            //selectedIDs += replaceAll(strValue, ',', '|');
            if (i < values.length - 1) {
                selectedIDs += ";";
            }
        }
        appSubDir = appSubDir != '' ? '/' + appSubDir : '';
        $.ajax(
            {
                type: 'POST',
                url: appSubDir + '/Fwk/SetSelectedIDs',
                dataType: 'html',
                cache: false,
                async: true,
                data: { selectedIDs: '' + selectedIDs + '', gridKey: '' + gridKey + '' },
                success: function () {
                    //Do Nothing
                }
            });
    } finally { /*alert(selectedIDs); return selectedIDs;*/ }
    //e.customArgs["selectedIDs"] = selectedIDs;
}

function replaceAll(str, find, replace) {
    return str.split(find).join(replace)
}

/*function OnCustomButtonClick(s, e) {
    switch (e.buttonID) {
        case 'Delete': s.DeleteRow(e.visibleIndex);
            break;
        case 'Edit': s.StartEditRow(e.visibleIndex);
            break;
        case "NOVO": s.AddNewRow();
    }
}*/

function BeginCallbackAccept(s, e, urlLink, target) {
    e.processOnServer = confirm('Confirmar?');
    if (e.processOnServer && urlLink != "") {
        if (target == 'parent' || target == '_parent') parent.location.href = urlLink;
        else if (target == 'blank' || target == '_blank') window.open(urlLink);
        else document.location.href = urlLink;
    }
}

function gridView_ExpandClick(s, e) {
    var index = s.GetFocusedRowIndex();
    if (s.Expanded) //TODO: Corrigir acesso à propriedade
        s.CollapseDetailRow(index);
    else
        s.ExpandDetailRow(index);
    $('html, body').animate({ scrollTop: $("#detailAction" + index).offset().top }, 1000);
    //s.SetMasterRowExpanded(index, !s.GetMasterRowExpanded(index));
}

function goToRodape(height) {
    //timer2 = setTimeout("goToRodape(1)", 5000);
    $(document).ready(function () {
        if (height == 0) {
            $("html, body").animate({ scrollTop: 0 }, 20000); //Apenas sobe
        }
        else if (height == 1) { //Rola a tela quando o tamanho excede a tela e depois sobe novamente
            $("html, body").animate({ scrollTop: $(document).height() }, 20000);
            $("html, body").animate({ scrollTop: 0 }, 5000);
        }
        else { /*-1 = Não executa ações */ }
    });
}

function scrollToAnchor(aid) {
    $('html, body').animate({ scrollTop: $("#" + aid).offset().top }, 1000);
}

function execTimer(seg, pageControl) {
    timer = setTimeout("execTimer(" + seg + ")", seg);
    //setTimeout("goToRodape(@activeRoll)", 30000);
    setTimeout("goToRodape(1)", 30000);
    var activeTab = pageControl.GetActiveTabIndex();
    //alert(activeTab);
    activeTab = (activeTab == 2) ? 0 : activeTab + 1;
    pageControl.SetActiveTabIndex(activeTab);
}

function stopTest() {
    clearTimeout(timer);
}

//function OnBeginCallback(s, e) {
//    var command = e.command; //GROUP / UNGROUP
//    var column = groupedColumn;
//    debugger;
//}

function OnColumnGrouping(s, e) {
    groupedColumns = "";
    var jsonObj = typeof s.cpGroupedRows == 'string' ? JSON.parse(s.cpGroupedRows) : s.cpGroupedRows;
    jsonObj.data.forEach(function (elemento) {
        groupedColumns += elemento.name + ";";
    });
    //groupedColumns += e.column.fieldName + ";";
}

function OnColumnUnGrouping(s, e) {
    groupedColumns = groupedColumns.replace(e.column.fieldName + ";", "");
}

function formExportClick(formName, exportFormat) {
    document.forms[formName].OutputFormat.value = exportFormat;
    if (groupedColumns != "" && groupedColumns != "undefined")
        document.forms[formName].Agrupadores.value = groupedColumns;
    //if(exportFormat == 'TXT'){
    //    alert('Exportando TXT');
    //    ""+formName+"".ExportToText();
    //    return false;
    //}
}

function GetChild(valorfiltro, inputname, urlload) {
    var procemessage = "<option value='0'> Aguarde carregando...</option>";
    $(inputname).html(procemessage).show();
    var url = urlload;

    $.ajax({
        url: url,
        data: { parentid: valorfiltro },
        cache: false,
        type: "POST",
        success: function (data) {
            var markup = "<option value='%'>Todos</option>";
            for (var x = 0; x < data.length; x++) {
                markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";
            }
            $(inputname).html(markup).show();
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}

function selecionarRegPopup(id, value, codField, descField) {
    parent.top.$("input#" + codField).val(id);
    parent.top.$("input#" + descField).val(value);
    //close pop
}

function loadPartialView(action, div, id) {
    $.get(action + id, function (id) {
        $("#" + div).html(id);
    });
}

function confirmAction(question) {
    var r = confirm(question);
    return r;
}

function setInputMask(tipo, field) {
    if (tipo == 'CPF') {
        $(function () {
            $(field).val('');
            $(field).unmask();
            $(field).mask('999.999.999-99');
            $(field).focus();
        });
    }
    else if (tipo == 'CNPJ') {
        $(function () {
            $(field).val('');
            $(field).unmask();
            $(field).mask('99.999.999/9999-99');
            $(field).focus();
        });
    }
}

function formatDate(valor, format) {
    var data = new Date(valor);
    var result = "";
    var tmp = "";
    for (var i = 0; i < format.length; i++) {
        tmp += format[i];
        if (tmp == "dd") {
            var dia = data.getDate();
            if (dia.toString().length == 1)
                dia = "0" + dia;
            result += dia;
            tmp = "";
        }
        if (tmp == "MM") {
            var mes = data.getMonth() + 1;
            if (mes.toString().length == 1)
                mes = "0" + mes;
            result += mes;
            tmp = "";
        }
        if (tmp == "yyyy") {
            var ano = data.getFullYear();
            result += ano;
            tmp = "";
        }
        if (tmp == "yy" && format[i + 1] != "y") {
            var ano = data.getYear();
            result += ano;
            tmp = "";
        }
        if (tmp == '/' || tmp == '-' || tmp == '-') {
            result += tmp;
            tmp = "";
        }
    }
    return result;
}

function applyGridFilter(filterExpression, grid, field, dateEdit1, dateEdit2, dateFormat, operation = '') {
    if (dateEdit2 != '')  //Interval
        filterExpression += ' [' + field + ']' + ' >= #' + formatDate(dateEdit1.GetValue(), dateFormat) + '# and ' + ' [' + field + ']' + ' <= #' + formatDate(dateEdit2.GetValue(), dateFormat) + '#';
    else {
        var toCompare = dateEdit1.GetValue();
        if (toCompare == null || toCompare.toString().startsWith(' '))
            toCompare = '';
        if (dateFormat != '' && toCompare != '')
            toCompare = formatDate(toCompare, dateFormat);
        filterExpression += ' Contains([' + field + '], \'' + toCompare + '\')';
    }
    grid.ApplyFilter(filterExpression);
    if (operation == '')
        operation = " And ";
    return filterExpression + " " + operation + " ";
}

function applyCptGridFilter(filterExpression, grid, field, dateEdit1, dateEdit2, dateFormat, operation = '') {
    var Cpt1 = new Date(dateEdit1.GetValue()).getMonth() + 1;
    var Cpt2 = new Date(dateEdit2.GetValue()).getMonth() + 1;
    var Exercicio = new Date(dateEdit1.GetValue()).getFullYear();
    //Monta o intervalo
    var CptStr = '';
    for (var i = Cpt1; i <= Cpt2; i++) {
        var mes = i;
        if (i.toString().length == 1)
            mes = "0" + i.toString();
        CptStr += '\'' + mes + '/' + Exercicio + '\'';
        if (i <= Cpt2 - 1)
            CptStr += ',';
    }
    var filterExpression = ' [' + field + ']' + ' In (' + CptStr + ') ';
    grid.ApplyFilter(filterExpression);
    if (operation == '')
        operation = " And ";
    return filterExpression + " " + operation + " ";
}

function applyPlainGridFilter(grid, fields, values, dtFormat) {
    var filterExpression = "";
    var value = values.split(",");
    var field = fields.split(",");
    if (dtFormat == '')
        dtFormat = 'dd/MM/yyyy';
    for (var i = 0; i < fields.length; i++) {
        var toCompare = value[i].split("#"); //Interval Ex: 01/06/2017#30/06/2017
        if (toCompare.length == 2)
            filterExpression += ' \'' + field[i] + '\'' + ' >= #' + formatDate(toCompare[0], dtFormat) + '# and ' + '\'' + field[i] + '\'' + ' <= #' + formatDate(toCompare[1], dtFormat) + '#';
        else
            filterExpression += ' Contains([' + field[i] + '], \'' + toCompare[0] + '\')';
        if (i < fields.length - 1)
            filterExpression += ' and ';
    }
    grid.ApplyFilter(filterExpression);
    return filterExpression;
}

function htmlEncode(value) {
    "use strict";
    //create a in-memory div, set it's inner text(which jQuery automatically encodes)
    //then grab the encoded contents back out.  The div never exists on the page.
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    "use strict";
    return $('<div/>').html(value).text();
}

function printDiv(el, scriptsLinks) {
    //var restorepage = document.body.innerHTML;
    var printcontent = document.getElementById(el).innerHTML;
    //document.body.innerHTML = printcontent;
    var htmlContent = '<html><head><title>Módulo de Impressão</title>';
    htmlContent += scriptsLinks;
    htmlContent += '</head><body>';
    htmlContent += printcontent;
    htmlContent += '</body><script>window.print(); window.close();</script></html>';
    var w = window.open();
    w.document.write(htmlContent);
    //setTimeout(w.print(), 4000);
    //document.body.innerHTML = restorepage;
}

function OnPopupInit(s, e) {
    var uiName = s.name;
    mainPopupControl[uiName] = s;
    iframe = mainPopupControl[uiName].GetContentIFrame();
    /* the "load" event is fired when the content has been already loaded */
    ASPxClientUtils.AttachEventToElement(iframe, 'load', OnContentLoaded);
}

//Métodos para MainPopup
function OnPopupShown(s, e) {
    if (showPopup)
        LoadingPanel.ShowInElement(iframe);
}

function OnContentLoaded(e) {
    showPopup = false;
    LoadingPanel.Hide();
}

function LoadMainPopupUrl(name, url, refreshGrid = '', height = 520, width = 940, title = '') {
    showPopup = true;
    if (refreshGrid != "") {
    }
    mainPopupControl[name].Show();
    mainPopupControl[name].SetHeaderText(title);
    mainPopupControl[name].SetContentUrl(url);
    mainPopupControl[name].SetHeight(height);
    mainPopupControl[name].SetWidth(width);
    mainPopupControl[name].Focus();
}

function OnButtonClick(s, e) {
    var uiName = s.name;
    showPopup = true;
    mainPopupControl[uiName].Show();
    mainPopupControl[uiName].SetContentUrl(url.GetText());
}

function scrollToAnchor(aid) {
    var aTag = $("#" + aid);
    $('html,body').animate({ scrollTop: aTag.offset().top }, 'slow');
}