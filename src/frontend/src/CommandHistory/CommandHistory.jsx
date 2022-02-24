import "./CommandHistory.scss"
import {Component} from "react";
import Message from "./Message"

class CommandHistory extends Component {
    render() {
        return (
            <div className={"commandHistory"}>
                <h2>Current data</h2>
                <div className="commandHistoryList">
                    <Message message={JSON.stringify(this.props.data, null, 2)} />
                </div>
            </div>
        );
    }
}

export default CommandHistory