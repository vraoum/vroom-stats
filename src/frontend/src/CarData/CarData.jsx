import "./CarData.scss"
import {Component} from "react";
import SpeedGauge from "./SpeedGauge/SpeedGauge";
import ThrottlePosition from "./ThrottlePosition/ThrottlePosition";

export default class CarData extends Component {
    render() {
        return (
            <div className="carData">
                <SpeedGauge data={this.props.data}/>
                <ThrottlePosition data={this.props.data} />
            </div>
        )
    }
}