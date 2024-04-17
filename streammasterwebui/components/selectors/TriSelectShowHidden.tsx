import { getTopToolOptions } from '@lib/common/common';
import { useShowHidden } from '@lib/redux/slices/useShowHidden';
import { Button } from 'primereact/button';
import { useCallback, useMemo } from 'react';

interface TriSelectProperties {
  readonly dataKey: string;
}

export const TriSelectShowHidden = ({ dataKey }: TriSelectProperties) => {
  const { showHidden, setShowHidden } = useShowHidden(dataKey);

  const getToolTip = useMemo((): string => {
    if (showHidden === null) {
      return 'All';
    }

    if (showHidden === true) {
      return 'Visible';
    }

    return 'Hidden';
  }, [showHidden]);

  const moveNext = useCallback(() => {
    if (showHidden === null) {
      setShowHidden(true);
      return;
    }

    if (showHidden === true) {
      setShowHidden(false);
      return;
    }

    setShowHidden(null);
  }, [setShowHidden, showHidden]);

  const getIcon = useMemo(() => {
    if (showHidden === null) {
      return 'pi pi-bullseye';
    }

    if (showHidden === true) {
      return 'pi pi-eye';
    }

    return 'pi pi-eye-slash';
  }, [showHidden]);

  return <Button icon={getIcon} onClick={() => moveNext()} rounded text tooltip={getToolTip} tooltipOptions={getTopToolOptions} />;
};
