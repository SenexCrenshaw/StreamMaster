import BaseButton, { type ChildButtonProps } from "./BaseButton";

const VisibleButton: React.FC<ChildButtonProps> = ({ disabled = false, iconFilled = true, label, onClick, tooltip = "Toggle Visibility" }) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-eye-slash"
      iconFilled={iconFilled}
      label={iconFilled !== true ? undefined : label ? label : undefined}
      onClick={onClick}
      tooltip={tooltip}
    />
  );
};

export default VisibleButton;
