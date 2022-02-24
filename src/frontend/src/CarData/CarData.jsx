import "./CarData.scss"
import {Component} from "react";
import SpeedGauge from "./SpeedGauge/SpeedGauge";
import ThrottlePosition from "./ThrottlePosition/ThrottlePosition";
import OilGauge from "./OilGauge/OilGauge";

export default class CarData extends Component {
    render() {
        return (
            <div className="carData">
                <SpeedGauge data={this.props.data}/>
                <ThrottlePosition data={this.props.data} />
                <OilGauge data={this.props.data} />
            </div>
        )
    }
}