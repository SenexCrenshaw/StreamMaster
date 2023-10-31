import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';
import { setShowSelectionsInternal } from './showSelectionsSlice';

export const useShowSelections = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setShowSelections = (selections: boolean | null | undefined) => {
    dispatch(
      setShowSelectionsInternal({
        selections,
        typename
      })
    );
  };

  const showSelections = useSelector((rootState: RootState) => rootState.showSelections[typename]);

  if (showSelections === undefined) {
    setShowSelections(null);
  }

  return { setShowSelections, showSelections };
};
