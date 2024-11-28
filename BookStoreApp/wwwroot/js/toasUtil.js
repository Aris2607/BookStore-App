function showToast(message, icon = 'success') {
    Swal.fire({
        toast: true,
        position: 'top-end',
        icon: icon,
        title: message,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
    });
}

function confirmAlert(title = "Are you sure?", text = "You won't be able to revert this!", confirmButtonColor = "#3085d6", confirmButtonText = "Yes, delete it!", confirmedTitle = "Deleted!", confirmedText = "Your file has been deleted.") {
    Swal.fire({
        title: title,
        text: text,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: confirmButtonColor,
        cancelButtonColor: "#d33",
        confirmButtonText: confirmButtonText
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: confirmedtitle,
                text: confirmedText,
                icon: "success"
            });
        }
    });
}