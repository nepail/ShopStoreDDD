
const MainProperties = {
    //訂單管理
    Order: {
        data: [
            {
                num: '',
                memberId: '',
                memberName: '',
                date: '',
                status: '',
                statusBadge: '',
                shippingMethod: '',
                shippingBadge: '',
                total: 0,
                isDel: 0,
                isPaid: 0
            }],

        postData: [
            {
                f_id: '',
                f_status: 0,
                f_shippingMethod: 0
            }],


        OrderStatus: {
            //0
            cartgoState: {
                '0306001': { code: '0306001', name: '待確認', style: 'bg-info' },
            },
            //1
            sipState: {
                '0306101': { code: '0306101', name: '郵寄', style: 'bg-green' },
            }
        },
    },

    //帳號管理
    User: {
        data: [{
            id: 0,
            name: '',
            account: '',
            groupId: 0,
            groupName: 'Admin',
            createTime: '',
            updateTime: '',
        }],

        group: {
            '1': 'Admin',
            '2': 'Normal'
        },


        postData: {

        }
    },

    Member: {
        data: [{
            id: 0,
            name: '',
            account: '',
            level: 0,
            money: 0,
            isSuspend: 0,
        }],


        GetStatusCSS: function (statusCode) {

            const STATUS = {
                LV1: 1,
                LV2: 2,
                LV3: 3,
                LV4: 4,
                LV5: 5,
                LV6: 6
            }

            const statement = {
                [STATUS.LV1]: 'bg-info',
                [STATUS.LV2]: 'bg-primary',
                [STATUS.LV3]: 'bg-success',
                [STATUS.LV4]: 'bg-danger',
                [STATUS.LV5]: 'bg-secondary',
                [STATUS.LV6]: 'bg-warning'
            }

            return statement[statusCode]

        },




    }
}

class InteractiveChatbox {
    constructor(a, b, c) {
        this.args = {
            button: a,
            chatbox: b
        }
        this.icons = c;
        this.state = false;
    }

    display() {
        const { button, chatbox } = this.args;        
        //button.addEventListener('click', () => this.toggleState(chatbox))

        button.click(() => {
            this.toggleState(chatbox);
        })
    }

    toggleState(chatbox) {
        
        this.state = !this.state;        
        this.showOrHideChatBox(chatbox, this.args.button);
    }

    showOrHideChatBox(chatbox, button) {
        if (this.state) {
            //chatbox.classList.add('chatbox--active');
            chatbox.addClass('chatbox--active');
            //this.toggleIcon(true, button);
        } else if (!this.state) {
            //chatbox.classList.remove('chatbox--active')
            chatbox.removeClass('chatbox--active');
            //this.toggleIcon(false, button)
        }
    }

    toggleIcon(state, button) {
        const { isClicked, isNotClicked } = this.icons;
        //let b = button.children[0].innerHTML;
        let b = button.children()[0].innerHTML;        

        if (state) {
            button.children()[0].innerHTML = isClicked;
        } else if (!state) {
            button.children()[0].innerHTML = isNotClicked;
        }
    }
}

