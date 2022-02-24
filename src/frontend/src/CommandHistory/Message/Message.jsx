import "./Message.scss"
import {Component} from "react";

class Message extends Component {

    render() {
        return <div className="message">
            <pre className="messageText">{this.props.message}</pre>
        </div>
    }
}

export default Message;