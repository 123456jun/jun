/*
 * @Author: your name
 * @Date: 2020-04-15 09:23:44
 * @LastEditTime: 2020-06-21 19:46:33
 * @LastEditors: Please set LastEditors
 * @Description: In User Settings Edit
 * @FilePath: \robot\src\utils\WmlMsg.js
 */
const WmlMsg = {}
const msgType = {
  info: 'info',
  success: 'success',
  warning: 'warning',
  error: 'error'
}
const icon = {
  success: 'iconfont icon-chenggong',
  info: 'iconfont icon-wenhao',
  danger: 'iconfont icon-warning',
  warning: 'iconfont icon-Danger-'
}

const className = {
  success: 'wmlmsg-success',
  info: 'wmlmsg-info',
  danger: 'wmlmsg-danger',
  warning: 'wmlmsg-warning'
}

WmlMsg.install = function (Vue) {
  Vue.prototype.$WmlMsg = function (message, type = 'success', close = true) {
    let data = {}
    if (typeof message === 'object') {
      data = { ...message }
    } else {
      data = {
        message: message,
        showClose: close,
        type: msgType[type],
        duration: 1800,
        iconClass: icon[type],
        customClass: className[type]
      }
      this.$message.closeAll()
      return this.$message(data)
    }
  }
}

