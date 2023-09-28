import { useDispatch, useSelector } from 'react-redux';
import { type AppDispatch, type RootState } from '../../../lib/redux/store';
import { setShowHiddenInternal } from './showHiddenSlice';

export const useShowHidden = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();


  const setShowHidden = (hidden: boolean | null | undefined) => {
    dispatch(setShowHiddenInternal({
      hidden: hidden,
      typename,
    }));
  };

  const showHidden = useSelector((rootState: RootState) => rootState.showHidden[typename]);

  if (showHidden === undefined) {
    setShowHidden(null);
  }

  return { setShowHidden, showHidden };
};
