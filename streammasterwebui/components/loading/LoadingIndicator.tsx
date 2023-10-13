import classNames from 'classnames';
import PropTypes from 'prop-types';
import styles from './LoadingIndicator.css';

type LoadingIndicatorProps = {
  className?: string;
  rippleClassName?: string;
  size?: number;
};

function LoadingIndicator({ className, rippleClassName, size }: LoadingIndicatorProps) {
  const sizeInPx = `${size}px`;
  const width = sizeInPx;
  const height = sizeInPx;

  return (
    <div className={className} style={{ height }}>
      <div className={classNames(styles.rippleContainer, 'followingBalls')} style={{ width, height }}>
        <div className={rippleClassName} style={{ width, height }} />

        <div className={rippleClassName} style={{ width, height }} />

        <div className={rippleClassName} style={{ width, height }} />
      </div>
    </div>
  );
}

LoadingIndicator.propTypes = {
  className: PropTypes.string,
  rippleClassName: PropTypes.string,
  size: PropTypes.number,
};

LoadingIndicator.defaultProps = {
  className: styles.loading,
  rippleClassName: styles.ripple,
  size: 50,
};

export default LoadingIndicator;
