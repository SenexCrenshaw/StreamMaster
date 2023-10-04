import BaseButton, { type ChildButtonProps } from './BaseButton'

const XButton: React.FC<ChildButtonProps> = ({
  disabled = true,
  onClick,
  tooltip = 'Remove',
  iconFilled,
}) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-times"
      iconFilled={iconFilled}
      onClick={onClick}
      severity="danger"
      tooltip={tooltip}
    />
  )
}

export default XButton
