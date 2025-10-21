function mostrarMensagemSucesso(texto) {
    mostrarMensagem(texto, $('#mensagemSucesso'));
}
function mostrarMensagemErro(texto) {
    mostrarMensagem(texto, $('#mensagemErro'));
}
function mostrarMensagemAviso(texto) {
    mostrarMensagem(texto, $('#mensagemAviso'));
}
function mostrarMensagemInfo(texto) {
    mostrarMensagem(texto, $('#mensagemInfo'));
}

function mostrarMensagem(texto, mensagemElemento, tempoSaida) {
    if (!tempoSaida)
        tempoSaida = 3000;
    mensagemElemento.text(texto);
    mensagemElemento.removeClass('d-none').hide().fadeIn(500);
    setTimeout(function () {
        mensagemElemento.fadeOut(500, function () {
            mensagemElemento.addClass('d-none');
        });
    }, tempoSaida);
}