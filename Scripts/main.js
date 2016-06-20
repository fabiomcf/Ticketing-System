$(document).ready(function () {
    $.ajax({
        url: "Helpdesk/Home/GetAdmin",
        //dataType: "json",
        type: "GET",
        data: { username_: $("#User").val() },
        //dataFilter: function (data) { return data; },
        success: function (output) {
            if (output == "True") {
                $("#adminlink").show();
            } else {
                $("#adminlink").hide();
            }
        }
    });
    //if ($("#username").val())

    $("#AdminMenu").hide();
    $("#Ticketsabertos").hide();
    $("#Ticketsfechados").hide();
    $("#result").hide();
    $("#Aindexform").live("click",function (evt) {
        evt.preventDefault();
        $(".featured .content-wrapper").slideUp();
        $("#AdminMenu").hide();
        $("#result").fadeOut();
        $("#Ticketsabertos").hide();
        $("#Ticketsfechados").hide();
        $("#indexform").show();
        $(".featured .content-wrapper").slideDown();
        return false;
    });

    $("form").submit(function (evt) {
        evt.preventDefault();
        if (confirm("Tem a certeza que pretende enviar? Após registar a ocorrência, terá que aguardar 2 minutos para fazer um novo registo!")) {
            $("#SaveTicket").prop('disabled', true);
            $("#result").removeClass();
            $("#result").show();
            $("#result").html("<h4 style='color:#fff;text-decoration: blink;'>A enviar...</h4>");
           
            $.ajax({
                url: "Helpdesk/Home/Save",
                data: {
                    username: $("#User").val(),
                    hostname: $("#Host").val(),
                    ipadrs: $("#IP").val(),
                    loc: $("#Location").val(),
                    date: $("#Date").val(),
                    //report: $("#Report").val(),
                    report: $("input:radio[name=Report]:checked").val(),
                    descript: $("#Description").val()
                },
                //dataType: "json",
                type: "POST",
                //dataFilter: function (data) { return data; },
                success: function (output) {
                    $('form')[0].reset();
                    $("#result").fadeIn();
                    if (output == 1) {
                        $("#result").removeClass().addClass("result_success").text("Registo efectuado com sucesso!");
                    } else {
                        $("#result").removeClass().addClass("result_error").text("ERRO: " + output);
                    }

                    console.log(output);
                }
            });
        }
        setTimeout(function () { $('#SaveTicket').prop('disabled', false) }, 120000);
        return false;
    });

   

    function DisplayError(xhr) {
        var msg = xhr.responseText;
        return msg;
    }

    $("#Aticketsabertos").live("click", function (evt) {
        $(".featured .content-wrapper").slideUp();
        $("#AdminMenu").hide();
        var result = "";
        $("#result").fadeOut();
        evt.preventDefault();
        
        $("#indexform").hide();
        $("#Ticketsabertos").show();
        $("#Ticketsfechados").hide();
        $(".featured .content-wrapper").slideDown();
     //   var obj1 = $("#User").val();
        $.ajax({
            cache: false,
            type: "GET",
            url: "Helpdesk/Home/TicketsAbertos",
            data: { username_ : $("#User").val() },
            dataType: "json",
            success: function (output) {
                $("#result").show();
                console.log(output);
                $.each(output, function (i, item) {
                        if (item.DB_RESULT != "") {
                            result += item.DB_RESULT;
                        } else {
                            result += "<tr>" +
                                "<td>" + item.ID + "</td>" +
                                "<td>" + item.Report + "</td>"+
                                "<td>" + item.Description + "</td>" +
                                "<td>" + item.Location + "</td>" +
                                "<td>" + item.Date + "</td>" +
                                "<td class='statusaberto'>" + item.Status + "</td>" +
                                "</tr>"
                            ;
                        };
                }),
                    $("#result").removeClass().addClass("result_table").fadeIn().html("<table><tr>"+
                        "<th>ID</th><th>Tipo</th><th>Descrição</th><th>Local</th><th>Data</th><th>Status</th>" + result + "<table>");
                
            },
            error: function (xhr, textStatus, jqXHR) { 
                $("#result").fadeIn();
                $("#result").removeClass().addClass("result_error").text("ERRO:" + xhr.status + " " + xhr.statusText + " " + DisplayError(xhr) + " "+jqXHR.responseText);
                console.log(DisplayError(xhr));
            }
         });
        return false;
    });
    $("#Aticketsresolvidos").live("click", function (evt) {
        $(".featured .content-wrapper").slideUp();
        $("#AdminMenu").hide();
        
        var result = "";
        $("#result").fadeOut();
        evt.preventDefault();
        
        $("#indexform").hide();
        $("#Ticketsabertos").hide();
        $("#Ticketsfechados").show();
        $(".featured .content-wrapper").slideDown();
        console.log("clicked");
        $.ajax({
            cache: false,
            type: "GET",
            url: "Helpdesk/Home/TicketsResolvidos",
            data: { username_: $("#User").val() },
            dataType: "json",
            success: function (output) {
                $("#result").show();
                $.each(output, function (i, item) {
                    if (item.DB_RESULT != "") {
                        result += item.DB_RESULT;
                    } else {
                        result += "<tr>" +
                            "<td>" + item.ID + "</td>" +
                            "<td>" + item.Report + "</td>" +
                            "<td>" + item.Description + "</td>" +
                            "<td>" + item.Location + "</td>" +
                            "<td>" + item.Date + "</td>" +
                            "<td class='statusfechado'>" + item.Status + "</td>" +
                            "</tr>"
                        ;
                    };
                });
                $("#result").removeClass().addClass("result_table").fadeIn().html("<table><tr>" +
                    "<th>ID</th><th>Tipo</th><th>Descrição</th><th>Local</th><th>Data</th><th>Status</th>" + result + "<table>");

             
            },
            error: function (xhr, textStatus, jqXHR) {
                $("#result").fadeIn();
                $("#result").removeClass().addClass("result_error").text("ERRO:" + xhr.status + " " + xhr.statusText + " " + DisplayError(xhr) + " " + jqXHR.responseText);
                console.log(DisplayError(xhr));
            }
        });
        return false;
    });

    $("#adminlink").live("click", function (evt) {
        $(".featured .content-wrapper").slideUp();
        evt.preventDefault();
        var result = "";
        $("#result").fadeOut();
        $("#indexform").hide();
        $("#Ticketsabertos").hide();
        $("#Ticketsfechados").hide();
        $("#AdminMenu").show();
        
        $(".featured .content-wrapper").slideDown();
        console.log("clicked");
        $.ajax({
            url: "Helpdesk/Home/GetAdminMenu",
            type: "POST",
            dataType: "json",
            success: function (output) {
                $("#Adminresult").fadeIn();
                $.each(output, function (i, item) {
                    if (item.DB_RESULT != "") {
                        result += item.DB_RESULT;
                    } else {
                        result += "<tr>" +
                            "<td id='ID_'>" + item.ID + "</td>" +
                            "<td id='in_ch_'>" + item.Report + "</td>" +
                            "<td id='username_'>" + item.User + "</td>" +
                            "<td>" + item.Host + "</td>" +
                            "<td>" + item.IP + "</td>" +
                            "<td>" + item.Description + "</td>" +
                            "<td>" + item.Location + "</td>" +
                            "<td>" + item.Date + "</td>" +
                            "<td class='status'>" + item.Status +
                            "<span id='sendmsg' class='ui-icon ui-icon-mail-closed' ></span>" +
                            "<span id='changestatus' class='ui-icon ui-icon-pencil' ></span>" +
                            "</td>" +
                            "</tr>";
                    };
                });
                var tabletdCount = $('#admintable tr').find('td').length;
                if (tabletdCount < 1)
                {
                    $('#admintable tr:last').after('<tr><td>Vai para casa, que já trabalhaste o suficiente!</td></tr>');
                }
                $("#Adminresult").removeClass().addClass("result_table").fadeIn().html("<table class='admintable'><tr>" +
                    "<th>ID</th><th>Tipo</th><th>User</th><th>Hostname</th><th>IP</th><th>Descrição</th><th>Local</th><th>Data</th><th>Status</th>" + result + "<table>");
            },
            error: function (output) {
                $("#Adminresult").fadeIn();
                $("#Adminresult").removeClass().addClass("result_error").text("ERRO:" + output.status + " " + output.statusText);
                console.log(output.status);
            }
        });
        return false;
    });
    $("#changestatus").live("click", function (evt) {
       if (confirm("Tem a certeza que pretende fechar o ticket?")) {
            evt.preventDefault();
            $.ajax({
                url: "Helpdesk/Home/ChangeStatus",
                data: {
                    ID: $(evt.target).closest("td").prev("td").prev("td").prev("td").prev("td").prev("td").prev("td").prev("td").prev("td").text(),
                    Username: $("#username_:last").text()
                },
                type: "POST",
                success: function (output) {
                    if (output == "true") {
                        $("#adminlink").click();
                    } 
                    console.log(output);
                }
            });
        }
        return false;
    });
    $("#sendmsg").live("click", function (evt) {
		console.log($("#ID_:last").text());
        var msg = prompt("Qual a mensagem a enviar ao utilizador?");
        if (confirm("Tem a certeza que pretende enviar a mensagem a solicitar abertura de novo ticket? AVISO: O ticket atual vai ser fechado automaticamente!")) {
            evt.preventDefault();
            $.ajax({
                cache: false,
                url: "Helpdesk/Home/SendMessageToUser",
                data: {
                    ID: $(evt.target).closest("td").prev("td").prev("td").prev("td").prev("td").prev("td").prev("td").prev("td").prev("td").text(),
                    IN_CH: $("#in_ch_:last").text(),
                    Username: $("#username_:last").text(),
                    Msg: msg
                },
                type: "POST",
                success: function (output) {
                    if (output == "true") {
                        $("#adminlink").click();
                    }
                    console.log(output + $("#in_ch:last").text());
                }
            });
        }
        return false;
    }); 
})