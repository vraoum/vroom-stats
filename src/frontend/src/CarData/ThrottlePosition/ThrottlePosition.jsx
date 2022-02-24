import "./ThrottlePosition.scss"
import {Component} from "react";

export default class ThrottlePosition extends Component {
    render() {
        return (
            <div>
                Throttle
                <div className={"throttlePosition"}>
                    <div className={"throttleValue"} style={{
                        height: this.props.data.throttlePosition??0 + '%',
                        background: '#8cef57'
                    }}>
                        {this.props.data.throttlePosition}%
                    </div>

                </div>
            </div>
        )
    }
}