var usuarioId = null;

$('.btn-resetar-usuario').on('click', function () {
    const nome = $(this).data('nome');
    usuarioId = $(this).data('id');
    $('#modalConfirmacaoLabel').text("Deseja resetar o e-mail?");
    $('#textoConfirmacao').html(`<p>Deseja resetar o usuário "${nome}"?</p> <p>O usuário irá receber um e-mail com link de reset.</p>`);
    const modal = new bootstrap.Modal($('#modalConfirmacao')[0]);
    modal.show();
});

$('#btnConfirmacaoSim').on('click', function () {
    if (usuarioId !== null) {
        resetarUsuario(usuarioId);
    }
    usuarioId = null;
    $('#modalConfirmacao').modal('hide');
});

$('#btnConfirmacaoNao').on('click', function () {
    usuarioId = null;
    $('#textoConfirmacao').text('');
});

function resetarUsuario(id, type) {
    if (!type)
        type = "POST"
    $.ajax({
        url: "/Admin/ResetarUsuario/",
        type: type,
        contentType: "application/x-www-form-urlencoded",
        data: { id: id },
        success: function (response) {
            mostrarMensagemSucesso("Usuário resetado com sucesso!");s
        },
        error: function (jqXHR, textStatus, errorThrown) {
            mostrarMensagemErro("Erro ao resetar usuário: Erros: \n-" + textStatus + " \n- " + errorThrown);
        }
    });
}