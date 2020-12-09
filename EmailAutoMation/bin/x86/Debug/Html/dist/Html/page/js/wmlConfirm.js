/*
 * @Author: vince.Liang
 * @Date: 2020-06-18 11:26:11
 * @LastEditTime: 2020-06-21 19:31:42
 * @LastEditors: Please set LastEditors
 * @Description: 动态弹窗方法，注意，要先调用$addpenFontFile方法来引入字体文件
 * @FilePath: \page\js\wmlConfirm.js
 */

/**
 * @author: vince.Liang
 * @function: 引入字体文件
 * @param {String} url 字体文件路径
 * @return: undefined
 */
function $addpenFontFile (url = '../iconfont/iconfont.css') {
  var head = Wml('head');
  var link = document.createElement('link');
  link.type = 'text/css';
  link.rel = 'stylesheet';
  link.href = url;
  head.appendChild(link);
}

/**
 * @author: vince.Liang
 * @function: 动态生成弹窗
 * @param {Object} object 配置选项
 * {
 *  type{String}:标识弹窗状态，success:成功;loading:加载中;info:提示;fail:失败,
 *  content{String}: 弹窗提示信息,
 *  time{Number}:自动关闭时间，传0则不自动关闭
 *  }
 * @return: undefined
 */
function $wmlConfirm ({ type = 'success', content = '您的操作已经成功!', time = 3000 }) {
  $closeWmlConfirm()
  let confirmBoxElement = document.createElement('div')
  let iconElement = document.createElement('i')
  let infoElement = document.createElement('div')
  let closeElement = document.createElement('i')
  confirmBoxElement.appendChild(iconElement)
  confirmBoxElement.appendChild(infoElement)
  confirmBoxElement.appendChild(closeElement)

  confirmBoxElement.id = 'wmlConfirm'
  confirmBoxElement.style.cssText = `
  position:fixed;
  top:30px;
  left:0;
  right:0;
  margin:0 auto;
  padding:16px 10px;
  display:flex;
  align-items:center;
  max-width:380px;
  background-color:#1BC4BD;
  border:1px solid #38B3B4;
  color:#fff;
  transition: all .1s;
  transform: translateY(-30px)
  `

  window.setTimeout(() => {
    confirmBoxElement.style.transform = 'translateY(0px)'
  }, 0)

  iconElement.style.cssText = `
  font-size:38px;
  `

  infoElement.style.cssText = `
  margin:0 20px;
  `
  closeElement.id = "wmlCloseIcon"
  closeElement.style.cssText = `
  margin-left:auto;
  cursor:pointer;
  `
  iconElement.classList.add('iconfont')
  closeElement.classList.add('iconfont')
  closeElement.classList.add('icon-guanbi')
  bodyElement.appendChild(confirmBoxElement)

  if (type === 'success') {
    confirmBoxElement.style.backgroundColor = '#1BC4BD'
    confirmBoxElement.style.borderColor = '#38B3B4'
    iconElement.classList.add('icon-chenggong')
  }

  if (type === 'info') {
    confirmBoxElement.style.backgroundColor = '#FFA507'
    confirmBoxElement.style.borderColor = '#EEAE28'
    iconElement.classList.add('icon-tishi')
  }

  if (type === 'loading') {
    confirmBoxElement.style.backgroundColor = '#6993FF'
    confirmBoxElement.style.borderColor = '#7B93DD'
    iconElement.style.transition = 'all 1s'
    iconElement.style.transformOrigin = '50% 50%'
    iconElement.classList.add('icon-loading2')
    var angle = 360
    rotateTimer1 = window.setTimeout(() => {
      iconElement.style.transform = `rotate(360deg)`
    }, 0)
    rotateTimer2 = window.setInterval(() => {
      angle = angle + 360
      iconElement.style.transform = `rotate(${angle}deg)`
    }, 1000)
  }

  if (type === 'fail') {
    confirmBoxElement.style.backgroundColor = '#fa4a63'
    confirmBoxElement.style.borderColor = '#db5b72'
    iconElement.classList.add('icon-warning')
  }
  infoElement.innerHTML = content
  confirmBoxElement.setAttribute('attr_type', type)
  if (time !== 0) {
    autoClose = setTimeout(() => {
      $closeWmlConfirm()
    }, time)
  }
}

/**
 * @author: vince.Liang
 * @function: 关闭提示弹窗
 * @data: 2019-3-27
 * @param none 
 * @return: undefined
 */
function $closeWmlConfirm () {
  if (!Wml('#wmlConfirm')) {
    return
  }
  let confirmBoxElement = Wml('#wmlConfirm')
  let type = confirmBoxElement.getAttribute('attr_type')
  confirmBoxElement.style.transform = 'translateY(-30px)'
  window.setTimeout(() => {
    bodyElement.removeChild(confirmBoxElement)
    if (!!autoClose) {
      clearTimeout(autoClose)
      autoClose = null
    }
    if (type === 'loading') {
      clearTimeout(rotateTimer1)
      rotateTimer1 = null
      clearInterval(rotateTimer2)
      rotateTimer2 = null
    }
  }, 100)
}

window.onload = function () {
  window.Wml = document.querySelector.bind(document)
  window.rotateTimer1 = null
  window.rotateTimer2 = null
  window.bodyElement = Wml('body')
  window.autoClose = null
  console.log(bodyElement)
  bodyElement.addEventListener('click', function (e) {
    console.log(!Wml('#wmlConfirm'))
    if (!Wml('#wmlConfirm')) {
      return
    }
    let confirmBoxElement = Wml('#wmlConfirm')
    let type = confirmBoxElement.getAttribute('attr_type')
    if (e.target.id === 'wmlCloseIcon') {
      confirmBoxElement.style.transform = 'translateY(-30px)'
      window.setTimeout(() => {
        bodyElement.removeChild(confirmBoxElement)
        if (!!autoClose) {
          clearTimeout(autoClose)
          autoClose = null
        }
        if (type === 'loading') {
          clearTimeout(rotateTimer1)
          rotateTimer1 = null
          clearInterval(rotateTimer2)
          rotateTimer2 = null
        }
      }, 100)
    }
  }, false)
}