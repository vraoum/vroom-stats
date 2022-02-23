import "./CarData.scss"
import {Component} from "react";
import SpeedGauge from "./SpeedGauge/SpeedGauge";

export default class CarData extends Component {
    render() {
        return (
            <div className="carData" style={{width: "400px"}}>
                <SpeedGauge />
            </div>
        )
    }
}