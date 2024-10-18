import { getTopToolOptions } from '@lib/common/common';
import { useShowSelections } from '@lib/redux/hooks/showSelections';
import { TriStateCheckbox, type TriStateCheckboxChangeEvent } from 'primereact/tristatecheckbox';

interface TriSelectShowProperties {
  readonly dataKey: string;
}

export const TriSelectShowSelection = ({ dataKey }: TriSelectShowProperties) => {
  const { showSelections, setShowSelections } = useShowSelections(dataKey);

  const getToolTip = (value: boolean | null | undefined): string => {
    if (value === null) {
      return 'Show All';
    }

    if (value === true) {
      return 'Show Selected';
    }

    return 'Not Selected';
  };

  return (
    <TriStateCheckbox
      className="sm-tristatecheckbox"
      onChange={(e: TriStateCheckboxChangeEvent) => {
        setShowSelections(e.value);
      }}
      tooltip={getToolTip(showSelections)}
      tooltipOptions={getTopToolOptions}
      value={showSelections}
    />
  );
};
