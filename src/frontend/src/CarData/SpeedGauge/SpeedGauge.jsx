import "./SpeedGauge.scss"
import {Component} from "react";
import Gauge from "../../Components/Gauge/Gauge";

export default class SpeedGauge extends Component {
    render() {
        return (
            <div className="speedGauge">
                <Gauge
                    value={50}
                    unit={"kmh"}
                    ringColor={"linear-gradient(90deg, rgba(47,226,111,1) 0%, rgba(252,176,69,1) 30%, rgba(253,29,29,1) 100%)"}
                    ringValue={3500}
                    ringMax={7000}
                    ringGraduateInterval={1000}
                    ringGraduateDisplayFunction={(val) => val/1000}
                    ringWarningStart={5800}
                    ringWarningEnd={7000}
                    ringWarningColor={"#ff5a5a"}
                />
            </div>
        )
    }
}