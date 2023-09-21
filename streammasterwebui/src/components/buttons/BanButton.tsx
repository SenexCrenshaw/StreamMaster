import BaseButton, { type ChildButtonProps } from "./BaseButton";

const BanButton: React.FC<ChildButtonProps> = ({ className, disabled = false, onClick, tooltip = "" }) => {
  return (
    <BaseButton
      className={className}
      disabled={disabled}
      icon="pi-ban"
      iconFilled={false}
      onClick={onClick}
      tooltip={tooltip}
    />
  );
};

export default BanButton;
