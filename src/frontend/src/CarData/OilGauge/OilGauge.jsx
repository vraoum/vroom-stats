import "./OilGauge.scss"
import {Component} from "react";
import Gauge from "../../Components/Gauge/Gauge";

export default class OilGauge extends Component {
    render() {
        return (
            <div className="oilGauge">
                Oil Temperature
                <Gauge
                    value={this.props.data.speed}
                    unit={"°C"}
                    ringColor={"linear-gradient(90deg, rgba(47,226,111,1) 0%, rgba(252,176,69,1) 30%, rgba(253,29,29,1) 100%)"}
                    ringValue={this.props.data.oilTemperature}
                    ringMax={180}
                    ringGraduateInterval={45}
                    ringWarningStart={120}
                    ringWarningEnd={180}
                    ringWarningColor={"#ff5a5a"}
                />
            </div>
        )
    }
}