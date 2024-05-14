$("#roomRezervasionForm").on("submit", function () {
    $.ajax({
        url: '/ReservasionAction/',
        method: 'POST',
        data: $(this).serialize(),
        success: function (response) {
            if (response === "success") {
                $("#results").html("<div class='alert alert-success'>" +
                    "Rezervasyonunuz Başarı İle Tamalandı Teşekkür Ederiz" 
                    +"</div>");
            }else {
                $("#results").html("<div class='alert alert-danger'>" +
                    "Seçtiğiniz Tarihlerde Odamız Doludur" 
                    +"</div>");
            }
        },
        error: function (xhr, status, error) {
            alert("Error!");
        }
    });
});