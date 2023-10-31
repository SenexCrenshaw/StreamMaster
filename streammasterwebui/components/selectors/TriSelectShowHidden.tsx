import { getTopToolOptions } from '@lib/common/common';
import { useShowHidden } from '@lib/redux/slices/useShowHidden';
import { TriStateCheckbox, type TriStateCheckboxChangeEvent } from 'primereact/tristatecheckbox';
import { useMemo } from 'react';

interface TriSelectProperties {
  readonly dataKey: string;
}
export const TriSelectShowHidden = ({ dataKey }: TriSelectProperties) => {
  const { showHidden, setShowHidden } = useShowHidden(dataKey);

  const getToolTip = useMemo((): string => {
    if (showHidden === null) {
      return 'Show All';
    }

    if (showHidden === true) {
      return 'Show Visible';
    }

    return 'Show Hidden';
  }, [showHidden]);

  return (
    <TriStateCheckbox
      className="sm-tristatecheckbox"
      onChange={(e: TriStateCheckboxChangeEvent) => {
        setShowHidden(e.value);
      }}
      tooltip={getToolTip}
      tooltipOptions={getTopToolOptions}
      value={showHidden}
    />
  );
};
