import BaseButton from "./BaseButton";

type OKButtonProps = {
  iconFilled?: boolean;
  onClick: () => void;
  tooltip?: string;
}

const OKButton: React.FC<OKButtonProps> = ({ iconFilled = true, onClick, tooltip = '' }) => {
  return (
    <BaseButton
      icon="pi pi-check"
      iconFilled={iconFilled}
      label="OK"
      onClick={onClick}
      rounded
      severity="success"
      tooltip={tooltip}
    />
  );
};

export default OKButton;
