import { type ChildButtonProps } from "./BaseButton";
import BaseButton from "./BaseButton";

const AddButton: React.FC<ChildButtonProps> = ({ disabled = false, iconFilled, label, onClick, tooltip = 'Add' }) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-plus"
      iconFilled={iconFilled}
      label={iconFilled === true ? undefined : label}
      onClick={onClick}
      severity="success"
      tooltip={tooltip}
    />
  );
};

export default AddButton;
