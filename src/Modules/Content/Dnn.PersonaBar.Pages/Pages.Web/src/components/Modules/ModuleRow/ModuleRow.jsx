import React, { Component, PropTypes } from "react";
import GridCell from "dnn-grid-cell";
import { EditIcon, TrashIcon } from "dnn-svg-icons";
import Checkbox from "dnn-checkbox";
import RadioButtons from "dnn-radio-buttons";
import styles from "./style.less";
import Localization from "../../../localization";

class ModuleRow extends Component {
    render() {
        const { module, onDelete, onEditing, isEditingModule, showCopySettings, onCopyChange } = this.props;
        const editClassName = "extension-action" + (isEditingModule ? " selected" : "");
        return (
            /* eslint-disable react/no-danger */
            <div className={styles.moduleRow} >
                {showCopySettings &&
                    <GridCell columnSize={10}>
                        <Checkbox value={module.includedInCopy !== null ? module.includedInCopy : true} 
                        onChange={onCopyChange.bind(this, module.id, "includedInCopy")} />
                    </GridCell>
                }
                <GridCell columnSize={showCopySettings ? 25 : 45} >
                    {module.title}
                </GridCell>
                <GridCell columnSize={showCopySettings ? 25 : 45} >
                    {module.friendlyName}
                </GridCell>
                {!showCopySettings &&
                    <GridCell columnSize={10} >
                        <div className="extension-action" dangerouslySetInnerHTML={{ __html: TrashIcon }} onClick={onDelete.bind(this, module)}></div>
                        <div className={editClassName} onClick={onEditing.bind(this, module)} dangerouslySetInnerHTML={{ __html: EditIcon }}></div>
                    </GridCell>
                }
                {showCopySettings &&
                    <GridCell columnSize={40}>
                        <RadioButtons
                            id={module.id}
                            onChange={onCopyChange.bind(this, module.id, "copyType")}
                            options={[{ label: Localization.get("ModuleCopyType.New"), value: "0" },
                            { label: Localization.get("ModuleCopyType.Copy"), value: "1" },
                            { label: Localization.get("ModuleCopyType.Reference"), value: "2" }
                            ]}
                            value={module.copyType !== null ? module.copyType.toString() : "1"} />
                    </GridCell>
                }
            </div>
            /* eslint-enable react/no-danger */
        );
    }
}

ModuleRow.propTypes = {
    module: PropTypes.object.isRequired,
    isEditingModule: PropTypes.bool.isRequired,
    onDelete: PropTypes.func.isRequired,
    onEditing: PropTypes.func.isRequired,
    onCopyChange: PropTypes.func,
    showCopySettings: PropTypes.bool
};

export default ModuleRow;