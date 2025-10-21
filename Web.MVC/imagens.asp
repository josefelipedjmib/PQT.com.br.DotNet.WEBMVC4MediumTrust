<%

option explicit

Response.CodePage = 65001
Response.CharSet = "utf-8"

'Passando cabeçalho para JPEG, pois o retorno do arquivo será a imagem.
response.AddHeader "Content-type","image/jpeg"

dim fs, fo, x
dim fotosAlbum, dirImgs, dirImgLogo, imagens, imagemOriginal
dim fotoTotal, foto, fotoTamanho, fotoParte, fotoParteX, fotoQualidade, fotoFloor, fotoLargura, fotoAltura, fotoLogo, imagemLargura, imagemAltura, imagemOriginalLargura, imagemOriginalAltura
dim fotosTamanhos, imagemTamanhos, imagemOriginalTamanhos, intPXY, logoPadrao, imagemLarguraParte
dim logoUsa, fotoArredondaTamanho, fotoInterlace

'Configurações iniciais e Parâmetros
dirImgs = "D:\www\Master Som\2018\img\"
dirImgLogo = "D:\www\Master Som\2018\logo.png"
fotosAlbum = request.queryString("a")
foto = cInt(request.queryString("n"))
fotoTamanho = request.queryString("t")
fotoParte = request.queryString("p")
fotoQualidade= txtPegNum(request.queryString("q"),1)
fotoFloor = txtPegNum(request.queryString("f"),1)
fotoInterlace=txtPegNum(request.queryString("i"),1)
logoPadrao = txtPegNum(request.queryString("l"),1)

function ceil(num)
    if isNumeric(num) then
        if num <> int(num) then
            ceil = int(num)+1
        else
            ceil = int(num)
        end if
    else
        ceil = 0
    end if
end function

function txtPegNum(strT,intT)
	dim erLetra
	dim strNum
	set erLetra = new regexp
	erLetra.pattern = "[^0-9\.,-]*"
	erLetra.global = true
	strNum = erLetra.replace(strT," ")
	strNum = replace(strNum,"  ","/")
	strNum = replace(strNum," ","")
	strNum = replace(strNum,"/"," ")
	strNum = split(strNum," ")
	if intT <> 0 then
		strNum = join(strNum,"")
	end if
	txtPegNum = strNum
end function

'Nome do álbum para as fotos
if fotosAlbum="" then
	'Álbum padrão se o mesmo não for passado
	fotosAlbum="fotos/galeriateste"
end if

if dirImgs = "" then
	dirImgs = server.mapPath("img/"&fotosAlbum&"/")
else
	dirImgs = dirImgs & fotosAlbum & "/"
end if

'Ler os arquivos no diretório e organiza em órdem natural
set fs = server.CreateObject("Scripting.FileSystemObject")
if fs.folderExists(dirImgs) then
    dim extensao
    redim imagens(-1)
    set fo = fs.getFolder(dirImgs)
    ' loop para pegar arquivos jpg ou jpeg
    for each x in fo.files
        extensao = lCase(fs.GetExtensionName(x))
        if extensao = "jpg" or extensao = "jpeg" then
            redim preserve imagens(uBound(imagens)+1)
            imagens(uBound(imagens)) = x 'x.Name
        end if
    next
end if

set fo = nothing
set fs = nothing
	


' total de fotos
fotoTotal = cInt(uBound(imagens) + 1)
	
' foto a ser exibida e tamanho
dim testaTamanho, retornoTestaTamanho
testaTamanho = txtPegNum(fotoTamanho,0)
if isArray(testaTamanho) then
	if uBound(testaTamanho)>-1 then
		retornoTestaTamanho = testaTamanho(0)
	else
		retornoTestaTamanho = ""
	end if
end if

if retornoTestaTamanho<>"" then
    fotoTamanho = retornoTestaTamanho
else
    fotoTamanho = left(fotoTamanho,3)
end if
foto = foto - 1
if instr(fotoTamanho, "-")>0 then
	fotoTamanho=split(fotoTamanho,"-")
elseif fotoTamanho = "" then
	fotoTamanho="nrm"
end if

' usa logo?
logoUsa = false

' se vai querer pegar metade da foto
' metade 1, esquerda, ou 2, direita

if not isNumeric(fotoParte) or fotoParte > 2 or fotoParte < 0  then
	fotoParte=0
end if

' qualidade da foto
if fotoQualidade="" then
	fotoQualidade=75
end if




' arredonda tamnho no caso de usar foto dividida
' em partes
fotoArredondaTamanho = false
if isNumeric(fotoFloor) then
    if fotoFloor = 1 then
        fotoArredondaTamanho=true
    end if
end if

' se irá fazer interlace na foto
if fotoInterlace="" then
    fotoInterlace=1
end if


' Tamanhos padrões de foto
' primeiro indice do array define a altura da imagem, a qual a logo original seguiu para ter seu tamanho definido.

fotosTamanhos = Array(1200_
				,230,150_
				,900,600_
				,720,480_
				,760,450_
				,800,600_
				,1280,720_ 
				,1920,1080_
				)

' verificando se vai ser usado tamanho logoPadrao
' ou personalizado

fotoLogo=fotosTamanhos(0)
if isArray(fotoTamanho) then
	fotoLargura=fotoTamanho(0)
	fotoAltura=fotoTamanho(1)
elseif fotoTamanho="peq" or fotoTamanho="p" then
	fotoLargura=fotosTamanhos(1)
	fotoAltura=fotosTamanhos(2)
elseif fotoTamanho="nrm" then
	fotoLargura=fotosTamanhos(3)
	fotoAltura=fotosTamanhos(4)
elseif fotoTamanho="dvd" then
	fotoLargura=fotosTamanhos(5)
	fotoAltura=fotosTamanhos(6)
elseif fotoTamanho="wb1" then
	fotoLargura=fotosTamanhos(7)
	fotoAltura=fotosTamanhos(8)
elseif fotoTamanho="pc1" then
	fotoLargura=fotosTamanhos(9)
	fotoAltura=fotosTamanhos(10)
elseif fotoTamanho="7hd" then
	fotoLargura=fotosTamanhos(11)
	fotoAltura=fotosTamanhos(12)
elseif fotoTamanho="fhd" then
	fotoLargura=fotosTamanhos(13)
	fotoAltura=fotosTamanhos(14)
else
	fotoLargura=fotosTamanhos(3)
	fotoAltura=fotosTamanhos(4)
end if

' verifica se a foto esta no intervalo
if foto<0 or foto>fotoTotal then
	foto=0
end if



' pegando a imagem requerida junto ao tamanho

imagemOriginal = imagens(foto)

dim imgP: set imgP = Server.CreateObject("Persits.Jpeg")
imgP.open imagemOriginal
imagemLargura = imgP.OriginalWidth
imagemAltura = imgP.OriginalHeight


imagemTamanhos=imagemLargura&","&imagemAltura
imagemOriginalTamanhos = split(imagemTamanhos,",")


'Logo número de Posições X (colunas) e Y (linhas)
intPXY = 3
' logo padrao para a foto
if logoPadrao = "" then
    logoPadrao = 9
end if

if logoPadrao>0 and logoPadrao<10 then
	logoUsa=true
end if

' ajustando imagens
imagemOriginalLargura = imagemOriginalTamanhos(0)
imagemOriginalAltura = imagemOriginalTamanhos(1)

imagemAltura = fotoAltura
imagemLargura = round(imagemOriginalLargura * imagemAltura / imagemOriginalAltura)
if imagemLargura>fotoLargura then
	imagemLargura = fotoLargura
	imagemAltura = round(imagemOriginalAltura * imagemLargura / imagemOriginalLargura)
end if
imagemLarguraParte=imagemLargura
if fotoParte>0 then
	imagemLarguraParte = int(imagemLargura/2)
end if
if fotoArredondaTamanho then
	imagemLarguraParte = int(imagemLarguraParte/2)*2
elseif fotoFloor<>"" then
    if fotoFloor>10 then
        imagemLargura = fotoFloor
        imagemLarguraParte = imagemLargura
    end if
end if

' response.write fotosAlbum & "<br>" &fotoTamanho & "<br>" &foto & "<br>" &fotoParte & "<br>" &fotoQualidade & "<br>" &logoPadrao & "<br>" &fotoFloor & "<br>" &fotoInterlace & "<br>"
' response.end


dim imgPP: set imgPP = Server.CreateObject("Persits.Jpeg")
imgPP.open imagemOriginal
imgPP.width = imagemLargura
imgPP.height = imagemAltura

imgP.New imagemLarguraParte, imagemAltura, &Hff0000

fotoParteX=0
if fotoParte=2 then
	fotoParteX=imagemLarguraParte*-1
end if

imgP.Canvas.DrawImage fotoParteX, 0, imgPP



' se usa logo
if logoUsa then
    dim imgLOH, imgLOW, imagemLarguraL, imagemAlturaL, logoPadraoX, logoPadraoY

	if dirImgLogo = "" then
		dirImgs = server.mapPath("logo.png")
	end if

    dim imgLogo: set imgLogo = Server.CreateObject("Persits.Jpeg")
    imgLogo.open dirImgLogo
    imgLOW = imgLogo.OriginalWidth
    imgLOH = imgLogo.OriginalHeight
	imagemLarguraL = round(imgLOW * (imagemAltura / fotoLogo))
	imagemAlturaL = round(imgLOH * (imagemAltura / fotoLogo))
    logoPadraoX = (imagemLargura / (intPXY-1) * ((intPXY - 1 + logoPadrao) mod intPXY) - imagemLarguraL / (intPXY-1) * ((intPXY - 1 + logoPadrao) mod intPXY))
	logoPadraoY = (imagemAltura / (intPXY-1) * (ceil(logoPadrao / intPXY)-1) - imagemAlturaL / (intPXY-1) * (ceil(logoPadrao / intPXY)-1))
	if fotoParte=2 then
		logoPadraoX = logoPadraoX - imagemLargura / 2
	end if
    imgLogo.width = imagemLarguraL
    imgLogo.height = imagemAlturaL
    on error resume next
    imgLogo.OutputFormat = 1
    imgP.Canvas.DrawPNGBinary logoPadraoX, logoPadraoY, imgLogo.binary
    if err.number <> 0 then
        imgP.Canvas.DrawImage logoPadraoX, logoPadraoY, imgLogo
    end if
	set imgLogo = nothing
end if

' Cria as imagens e depois destroi os objetos usados.
if fotoInterlace = 1 then
    imgP.Progressive = True
else
    imgP.Progressive = False
end if
imgP.Quality = fotoQualidade
imgP.sendBinary
set imgP = nothing
set imgPP = nothing


%>